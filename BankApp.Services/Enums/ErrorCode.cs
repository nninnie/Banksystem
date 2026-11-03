namespace BankApp.Services.Enums
{
    // Result of a deposit, withdrawal or transfer
    public enum ErrorCode
    {
        OK,
        BalanceTooLow,
        IncorrectAmount,
        AccountNotFound
    }
}
