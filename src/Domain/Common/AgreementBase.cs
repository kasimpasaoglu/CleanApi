namespace Domain.Common;

public class AgreementBase : BaseAuditableEntity<Guid>
{
    public Guid CustomerId { get; set; } // InitialAgreement ile set edilir. (1)
    public Guid ProjectUnitId { get; set; } // InitialAgreement ile set edilir. (1)
    public Guid? SourceAppointmentId { get; set; }   // InitialAgreement ile set edilir. (1)
    public decimal? CurrentListPrice { get; set; }   //  InitialAgreement ile set edilir. (1)
    public decimal TotalPrice { get; set; }          //  InitialAgreement ile set edilir. (1)
    public int? MaturityCount { get; set; }        //FillPreAgreement ile set edilir. (2)
    public decimal? Deposit { get; set; }            //  FillPreAgreement ile set edilir. (2)
    public DateOnly? DepositDueDate { get; set; }
    public decimal? DownPayment { get; set; }        // FillPreAgreement ile set edilir. (2)
    public DateOnly? DownPaymentDueDate { get; set; }
    public DateOnly? AgreementDate { get; set; }      // FillPreAgreement / FillAgreements ile kullanıcıdan alınır. (2/4)
    public decimal? TotalInterimPayment { get; set; } // FillAgreements ile set edilir. (4)
}
