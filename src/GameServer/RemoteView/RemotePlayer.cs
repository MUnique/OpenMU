// <copyright file="RemotePlayer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.GameServer.RemoteView;

using System.Buffers;
using Microsoft.Extensions.Logging;
using MUnique.OpenMU.GameLogic;
using MUnique.OpenMU.GameLogic.Views;
using MUnique.OpenMU.GameServer.MessageHandler;
using MUnique.OpenMU.Network;
using MUnique.OpenMU.Network.PlugIns;
using MUnique.OpenMU.PlugIns;

/// <summary>
/// A player which is playing through a remote connection.
/// </summary>
public class RemotePlayer : Player, IClientVersionProvider
{
    private readonly byte[] _packetBuffer = new byte[0xFF];

    private ClientVersion _clientVersion;

    /// <summary>
    /// Initializes a new instance of the <see cref="RemotePlayer"/> class.
    /// </summary>
    /// <param name="gameContext">The game context.</param>
    /// <param name="connection">The remote connection.</param>
    /// <param name="clientVersion">The expected client version of the connected player.</param>
    public RemotePlayer(IGameServerContext gameContext, IConnection connection, ClientVersion clientVersion)
        : base(gameContext)
    {
        this.Connection = connection;
        this._clientVersion = clientVersion;
        this.MainPacketHandler = new MainPacketHandlerPlugInContainer(this, gameContext.PlugInManager, gameContext.LoggerFactory);
        this.MainPacketHandler.Initialize();
        this.Connection!.PacketReceived += this.PacketReceivedAsync;
        this.Connection!.Disconnected += this.DisconnectAsync;
    }

    /// <inheritdoc />
    public event EventHandler? ClientVersionChanged;

    /// <summary>
    /// Gets the game server context.
    /// </summary>
    public IGameServerContext GameServerContext => (IGameServerContext)this.GameContext;

    /// <inheritdoc />
    public ClientVersion ClientVersion
    {
        get => this._clientVersion;

        set
        {
            if (value == this.ClientVersion)
            {
                return;
            }

            this._clientVersion = value;
            this.ClientVersionChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Gets the connection.
    /// </summary>
    internal IConnection? Connection { get; private set; }

    /// <summary>
    /// Gets the currently effective appearance serializer.
    /// </summary>
    internal IAppearanceSerializer AppearanceSerializer => this.ViewPlugIns.GetPlugIn<IAppearanceSerializer>() ?? throw new Exception("No appearance serializer available.");

    /// <summary>
    /// Gets the currently effective item serializer.
    /// </summary>
    internal IItemSerializer ItemSerializer => this.ViewPlugIns.GetPlugIn<IItemSerializer>() ?? throw new Exception("No item serializer available.");

    /// <summary>
    /// Gets the main packet handler.
    /// </summary>
    /// <value>
    /// The main packet handler.
    /// </value>
    internal MainPacketHandlerPlugInContainer MainPacketHandler { get; }

    /// <inheritdoc />
    protected override bool IsPlayerStoreOpeningAfterEnterSupported => this.ClientVersion.Season > 100;

    /// <inheritdoc />
    protected override ICustomPlugInContainer<IViewPlugIn> CreateViewPlugInContainer()
    {
        return new ViewPlugInContainer(this, this.ClientVersion, this.GameContext.PlugInManager);
    }

    /// <inheritdoc/>
    protected override async ValueTask InternalDisconnectAsync()
    {
        await base.InternalDisconnectAsync().ConfigureAwait(false);
        if (this.Connection is { Connected: true })
        {
            await this.Connection.DisconnectAsync().ConfigureAwait(false);
            this.Connection.Dispose();
            this.Connection = null;
        }
    }

    /// <summary>
    /// Is getting called when a packet got received from the connection of the player.
    /// </summary>
    /// <param name="sequence">The packet.</param>
    private async ValueTask PacketReceivedAsync(ReadOnlySequence<byte> sequence)
    {
        using var loggingScope = this.Logger.BeginScope(
            ("GameServer", this.GameServerContext.Id),
            ("Connection", this.Connection!),
            ("Account", this.GetAccountName()),
            ("Character", this.GetSelectedCharacterName()));

        Memory<byte> buffer = Memory<byte>.Empty;
        try
        {
            IMemoryOwner<byte>? owner = null;
            if (sequence.Length <= this._packetBuffer.Length)
            {
                sequence.CopyTo(this._packetBuffer);
                buffer = this._packetBuffer.AsMemory(0, this._packetBuffer.GetPacketSize());
            }
            else
            {
                owner = MemoryPool<byte>.Shared.Rent((int)sequence.Length);
                buffer = owner.Memory[..(int)sequence.Length];
                sequence.CopyTo(buffer.Span);
            }

            try
            {
                if (this.Logger.IsEnabled(LogLevel.Debug))
                {
                    this.Logger.LogDebug("[C->S] {0}", buffer.ToArray().AsString());
                }

                await this.MainPacketHandler.HandlePacketAsync(this, buffer).ConfigureAwait(false);
            }
            finally
            {
                owner?.Dispose();
            }
        }
        catch (Exception ex)
        {
            this.Logger.LogError(ex, $"Error while processing the message: {buffer.ToArray().AsString()}");
        }
    }

    private string GetAccountName()
    {
        var account = this.Account;
        if (account != null)
        {
            return account.LoginName;
        }

        return string.Empty;
    }

    private string GetSelectedCharacterName()
    {
        var character = this.SelectedCharacter;
        if (character != null)
        {
            return character.Name;
        }

        return string.Empty;
    }
}