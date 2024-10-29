namespace MUnique.OpenMU.GameLogic.Views.Duel;

public enum DuelStartResult
{
    Undefined,

    Success,

    Refused,

    FailedByError,

    FailedByTooLowLevel,

    FailedByNoFreeRoom,

    FailedByNotEnoughMoney,
}