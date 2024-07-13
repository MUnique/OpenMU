namespace MUnique.OpenMU.GameLogic.Views.Duel;

public enum DuelStartResult
{
    Success,

    Refused,

    FailedByError,

    FailedByTooLowLevel,

    FailedByNoFreeRoom,

    FailedByNotEnoughMoney,
}