// <copyright file="CustomTestPlugInContainer.cs" company="MUnique">
// Licensed under the MIT License. See LICENSE file in the project root for full license information.
// </copyright>

namespace MUnique.OpenMU.PlugIns.Tests
{
    using System;
    using System.Linq;

    /// <summary>
    /// A test implementation of a <see cref="CustomPlugInContainerBase{TPlugIn}"/>.
    /// </summary>
    public class CustomTestPlugInContainer : CustomPlugInContainerBase<ICustomTestPlugInContainer>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomTestPlugInContainer"/> class.
        /// </summary>
        /// <param name="manager">The plugin manager which manages this instance.</param>
        public CustomTestPlugInContainer(PlugInManager manager)
            : base(manager)
        {
            this.Initialize();
        }

        /// <summary>
        /// Gets or sets a value indicating whether this instance creates new plug ins in <see cref="CreatePlugInIfSuitable"/>.
        /// </summary>
        public bool CreateNewPlugIns { get; set; } = true;

        /// <inheritdoc />
        protected override void CreatePlugInIfSuitable(Type plugInType)
        {
            if (this.CreateNewPlugIns)
            {
                this.AddPlugIn(Activator.CreateInstance(plugInType) as ITestCustomPlugIn, true);
            }
        }

        /// <inheritdoc />
        protected override ICustomTestPlugInContainer DetermineEffectivePlugIn(Type interfaceType)
        {
            return this.ActivePlugIns.FirstOrDefault(interfaceType.IsInstanceOfType);
        }

        /// <inheritdoc />
        protected override bool IsNewPlugInReplacingOld(ICustomTestPlugInContainer currentEffectivePlugIn, ICustomTestPlugInContainer activatedPlugIn)
        {
            return true;
        }
    }
}