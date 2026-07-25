namespace Domain.Constants;

public abstract class ErrorCodes
{
    public abstract class NotFound
    {
        public const string Customer = "Müşteri Bulunamadı.";
        public const string Referrer = "Referans gösterilen müşteri bulunamadı.";
        public const string ContactRecord = "İletişim Kaydı Bulunamadı.";
        public const string TempCustomer = "Kayıt Bulunamadı!";

        public const string User = "Kullanıcı Bulunamadı.";

        public const string FindMe = "Kullanıcı doğrulanamadı.";

        public const string Agreement = "Sözleşme Bulunamadı.";
        public const string AgreementPdf = "Sözleşmeye ait PDF dosyası bulunamadı.";
        public const string PreAgreementPdf = "Ön Protokole ait PDF dosyası bulunamadı.";
        public const string AgreementDraft = "Sözleşme Onay Formu Bulunamadı";
        public const string AgreementActiveDraft = "Sözleşme Aktif Onay Formuna sahip değil";
        public const string ConstructionAgreement = "İnşaat Sözleşmesi Bulunamadı.";
        public const string ParentAgreementNotSigned = "Kaynak sözleşme henüz imzalanmamış.";
        public const string Deposit = "Depozito kaydı Bulunamadı.";
        public const string DownPayment = "Peşinat kaydı Bulunamadı.";
        public const string AgreementPayment = "Ödeme Kalemi Bulunamadı.";

        public const string AppointmentCancellationApproval = "Randevu İptal Onay Talebi Bulunamadı.";
        public const string Appointment = "Randevu Bulunamadı.";
        public const string SalesCallRecord = "Satış Görüşmesi Kaydı Bulunamadı.";

        public const string CallRecord = "Çağrı Kaydı Bulunamadı.";

        public const string CallTaskRequest = "Çağrı Görev Talebi Bulunamadı.";

        public const string CallTask = "Çağrı Görevi Bulunamadı.";

        public const string CustomerPortfolio = "Müşteri Portföyü Bulunamadı.";

        public const string UserBranch = "Kullanıcı Şubesi Bulunamadı.";

        public const string Department = "Departman Bulunamadı.";
        public const string Country = "Ülke Bulunamadı.";
        public const string City = "Şehir Bulunamadı.";
        public const string District = "İlçe Bulunamadı.";
        public const string Neighborhood = "Mahalle Bulunamadı.";

        public const string LeadSource = "Kaynak Bulunamadı.";
        public const string LeadSourceNote = "Kaynak Notu Bulunamadı.";
        public const string LeadStatus = "Lead Durumu Bulunamadı.";
        public const string LeadStatusNote = "Lead Durumu Notu Bulunamadı.";

        public const string Project = "Proje Bulunamadı.";
        public const string ProjectStage = "Proje Aşaması Bulunamadı.";
        public const string ProjectUnit = "Proje Birimi Bulunamadı.";

        public const string SurveyCallRecord = "Anket Çağrı Kaydı Bulunamadı.";
        public const string SurveyCallQuestion = "Anket Sorusu bulunamadı";
        public const string SurveyQuestionOption = "Anket Sorusu Seçeneği bulunamadı.";

        public const string HousingType = "Konut Tipi Bulunamadı.";
        public const string Job = "Meslek Bulunamadı.";

        public const string Ticket = "Talep Formu Bulunamadı.";
        public const string TicketActionHistory = "Talep Formu Hareket Geçmişi Bulunamadı.";
        public const string TicketAttachment = "Bu talep formuna ait dosya bulunamadı.";

        public const string Reminder = "Hatırlatıcı bulunamadı.";

        public const string AfterSalesContactRecord = "Satış Sonrası Kayıt Bulunamadı.";
        public const string AgencyContactRecord = "Acenta İletişim Kaydı Bulunamadı.";

        public const string SalesConsultantPoolEntry = "Satış Danışmanı Havuz Kaydı Bulunamadı.";

        public const string MarketingCampaign = "Pazarlama Kampanyası Bulunamadı.";

    }

    public abstract class Failure
    {
        public const string Roles = "Role: Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz..";
        public const string User = "User : Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz..";

        public const string TickerManager = "Zamanlayıcı: Beklenmedik bir hata oluştu, Lütfen tekrar deneyiniz..";
        public const string Ticket = "Ticket: Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz..";

        public const string CustomerPortfolioNotChanged = "Müşteri portföyünde değişiklik yapılırken hata oluştu. Lütfen tekrar deneyiniz..";

        public const string AgreementCustomer =" Sözleşme Müşterisi Bilgileri Eksik ya da Hatalı. Lütfen tekrar deneyiniz..";
    }

    public abstract class Reports
    {
        public const string ParamsJsonInvalid = "Rapor parametreleri JSON formatına uymuyor.";
        public const string ParamsRequired = "Rapor parametreleri eksik veya okunamadı.";
        public const string DateRangeRequired = "Rapor için başlangıç ve bitiş tarihleri zorunludur.";
        public const string DateRangeInvalid = "Bitiş tarihi başlangıç tarihinden önce olamaz.";
    }


    public abstract class AlreadyExists
    {
        public const string Customer = "Müşteri Zaten Mevcut.";
        public const string MetaLead = "Lead Zaten Mevcut.";
        public const string User = "Kullanıcı Zaten Mevcut.";
        public const string Department = "Departman Zaten Mevcut.";

        public const string LeadSource = "Kaynak Zaten Mevcut.";
        public const string LeadSourceNote = "Kaynak Notu Zaten Mevcut.";
        public const string LeadStatus = "Durum Zaten Mevcut.";
        public const string LeadStatusNote = "Durumu Notu Zaten Mevcut.";

        public const string Project = "Proje Zaten Mevcut.";
        public const string ProjectStage = "Proje Aşaması Zaten Mevcut.";

        public const string SurveyCallRecordForAppointment = "İlgili randevu için zaten bir anket çağrı kaydı mevcut.";
        public const string SurveyCallQuestion = "Anket sorusu zaten mevcut.";

        public const string PaymentPlan = "Ödeme Planı Zaten Mevcut.";

        public const string CustomerPhoneNumber = "Aynı telefon numarasına sahip bir müşteri zaten mevcut.";

        public const string Phone = "Telefon Numarası Zaten Mevcut.";

        public const string CallRecord = "Çağrı Kaydı Zaten Mevcut.";
        public const string Job = "Meslek Zaten Mevcut.";

        public const string SalesConsultantPoolEntry = "Bu kullanıcı zaten havuzda mevcut.";

        public const string ReportJob = "Aynı parametrelerle bekleyen bir rapor zaten kuyrukta.";

        public const string MarketingCampaign = "Aynı dış kampanya ID'si ile bir pazarlama kampanyası zaten mevcut.";
        public const string MarketingAd = "Aynı dış reklam ID'si ile bir pazarlama reklamı zaten mevcut.";

    }



    //Appointment Errors
    public const string AppointmentHasAgreement = "Randevunun sözleşme/sözleşmeleri bulunduğu için işlem yapılamaz.";
    public const string AppointmentIsInitialized = "Randevu zaten başlatılmış. İşlem yapılamaz.";
    public const string AppointmentHasOtherOpenAppointments = "Müşteri için başka açık randevu bulunuyor.";
    public const string AppointmentTypeCanNotEmpty = "Randevu tipi boş olamaz.";
    public const string NotAssignedAppointment = "Randevu, sözleşme oluşturmak için bir user'a atanmalıdır.";
    public const string AssignmentReasonNoteIsEmpty = "Randevu atama değişikliği atama nedeni notu boş olamaz.";
    public const string AppointmentCancelledOrCompleted = "Randevu zaten iptal edilmiş veya tamamlanmış durumda, işlem yapılamaz";
    public const string AppointmentStatusNotAvailable = "Randevu durumu bu işlem için uygun değil, işlem yapılamaz";
    public const string CannotBePlannedForPastDate = "Geçmiş tarih için planlama yapılamaz.";
    public const string SourceAppointmentIdEmpty = "Kaynak randevu seçilirken hata oluştu.Lütfen tekrar deneyin";
    public const string SourceAppointmentIdSameAsCurrent = "Kaynak randevu, mevcut kaynak randevu ile aynı. Lütfen farklı bir randevu seçiniz.";
    public const string AppointmentAlreadyChanged = "İlgili randevu işlem görmüş, değişiklik yapılamaz.";

    public const string SourceAppointmentCustomerNotMatchAgreementCustomer = "Seçilen kaynak randevu, sözleşmenin müşterisine veya müşterinin referansına ait olmalıdır.";
    public const string AppointmentSourceRequired = "Randevu kaynağı olarak yalnızca bir çağrı kaydı veya bir acenta iletişim kaydı seçilmelidir.";
    public const string AppointmentNotDeletableStatus = "Yalnızca planlanmış durumdaki randevular silinebilir.";
    public const string AppointmentHasConfirmationCall = "Randevuya ait teyit araması bulunduğu için silinemez.";
    public const string AppointmentLinkedToSourceRecord = "Randevu bir çağrı kaydı veya acenta iletişim kaydı ile ilişkili olduğu için silinemez.";
    public const string AppointmentIsSourceOfAgreement = "Randevu bir sözleşmenin kaynağı olduğu için silinemez.";
    //SalesCallRecord Errors
    public const string CustomerHasScheduledSalesRecord = "Müşterinin zaten planlanmış bir satış görüşmesi kaydı bulunmaktadır.";


    //Agreement Errors
    public const string AgreementDraftInvalidStatus = "Sözleşme onay formu yalnızca reddedilmiş durumda güncellenebilir.";
    public const string AgreementHasActiveDraft = "Sözleşmenin zaten aktif bir onay formu bulunmaktadır.";
    public const string DuplicatePaymentsEntries = "Aynı ödeme tipi ve vade tarihine sahip birden fazla ödeme girişi bulunmaktadır.";
    public const string PaymentsRequestMissing = "Ödeme bilgileri eksik.";
    public const string AgreementCannotBeCancelled = "Sözleşme iş kurallarI gereği iptal edilemez. Lütfen geçerli iptal koşullarını kontrol ediniz.";
    public const string AgreementEarlyReturnTimeExceeded = "Sözleşme erken iade süresi aşıldığı için iptal edilemez.";
    public const string CannotDeleteDraftFromSignedAgreement = "İmzalanmış sözleşmenin onay formu silinemez.";
    public const string PaymentTotalMismatch = "Ödeme toplamı sözleşme tutarı ile eşleşmiyor.";
    public const string CanNotWelcomeCall = "Welcome Araması kaydı oluşturulamaz! Sözleşme bulunamadı ya da daha önce Welcome Araması yapılmış olabilir. Lütfen kontrol ediniz.";

    public const string AgreementCanNotBeUpdate = "Bu alandan sadece sözleşmesi sisteme yüklenmiş ve iade edilmemiş sözleşmeler güncellenebilir.";
    public const string PaidPaymentCannotBeModified = "Ödendi olarak işaretlenmiş kalem düzenlenemez veya silinemez; önce ödendi işaretini kaldırın.";
    public const string DepositPaymentAlwaysPaid = "Kapora ödemesi her zaman tahsil edilmiş kabul edilir; işareti kaldırılamaz.";
    public const string AgreementInCancellationProcess = "İptal sürecindeki sözleşme üzerinde ödeme işlemi yapılamaz.";

    //ConstructionAgreement Errors
    public const string ProjectUnitAlreadyHasConstructionAgreement = "Bu konut için zaten aktif bir inşaat sözleşmesi var.";
    public const string ConstructionAgreementAlreadyConfirmed = "Sözleşme zaten onaylanmış, değişiklik yapılamaz.";
    public const string ConstructionAgreementPaymentsMissing = "Ödeme planı boş gönderilemez.";
    public const string ConstructionAgreementDuplicatePayments = "Aynı ödeme tipi ve vade tarihine sahip birden fazla ödeme girişi yapılamaz.";


    //CustomerPortfolio Errors
    public const string CustomerPortfolioAlreadyConfirmed = "Müşteri portföyü zaten onaylanmış";
    public const string CustomerPortfolioAlreadyAssigned = "Müşteri zaten bir kullacının portföyünde.";
    public const string CustomerPortfolioNotOwned = "Müşteri portföy yetkilisi bulunamadı. Yöneticiniz ile iletişime geçiniz";
    public const string CustomerPortfolioAssignmentFailed = "Müşteri portföyü ataması başarısız oldu.";
    public const string CustomerPortfolioDiscardUnauthorized = "Başka bir kullanıcıya ait müşteri portföyü discard edilemez.";
    public const string CustomerPortfolioHasSignedSales = "Müşteri portföyünde imzalanmış satışlar olduğu için işlem yapılamaz.";


    //Customer Errors
    public const string CustomerLockedByAnotherUser = "Müşteri kartı üzerinde başka bir kullanıcı tarafından işlem yapılıyor.";
    public const string CustomerUnlockUnauthorized = "Başka bir kullanıcı tarafından kilitlenmiş müşteri kartının kilidi açılamaz.";
    public const string CustomerEmailRequirement = "Müşteri email adresi eksik. İşlem başarısız.";
    public const string CustomerIbanAndTcRequirement = "Müşteri IBAN ve ya TC bilgisi eksik. İşlem başarısız.";
    public const string CustomerTcRequirement = "Müşteri TC bilgisi eksik. İşlem başarısız.";
    public const string SourceNoteNotChanged = "İlgili Kaynak Notu Değiştirilemez.Lütfen yöneticiniz ile iletişime geçiniz";
    public const string CustomerNotInPortfolio = "Seçilen müşteri sizin portföyünüzde değil. İlgili işlemi sadece kendi portföyünüzdeki müşterilere yapabilirsiniz.";
    public const string CustomerCannotReferItself = "Müşteri kendisini referans olarak gösteremez.";


    //ProjectUnit Errors
    public const string ProjectUnitUserIdNotMatch = "Başka bir kullanıcıya ait proje birimi üzerinde işlem yapılamaz";
    public const string ProjectUnitEmpty = "Proje birimi listesi boş olamaz.";
    public const string ProjectUnitNotSuitable = "Seçilen proje birimi işlem için uygun değil.";

    //Survey Errors
    public const string SurveyQuestionInUse = "Anket sorusu kullanımda olduğu için güncellenemez veya silinemez.";
    public const string SurveyQuestionAlreadyActive = "Anket sorusu zaten aktif.";


    //CallRecord
    public const string CallRecordDraftStaleBase = "Temel kayıt değiştiği için işlemlere devam edilemez. Lütfen sayfayı yenileyip tekrar deneyiniz.";
    public const string CallRecordDraftPendingExists = "Bu çağrı kaydı  için bekleyen bir onay formu zaten mevcut.";
    public const string CallRecordDraftInvalidData = "Çağrı kaydı verileri geçersiz.";
    public const string InterestedTagOrAppointmentExist = "Müşterinin ilgili etiketine sahip bir çağrı kaydı veya henüz tamamlanmamış bir randevusu bulunmaktadır.";

    //Reminder
    public const string ReminderNotOwned = "Bu hatırlatıcı üzerinde işlem yapma yetkiniz yok.";
    public const string ReminderCustomerHasOpenReminder = "Seçilen müşteriye ait açık bir hatırlatıcınız zaten var. Bu hatırlatıcıyı yeniden açmadan önce mevcut açık hatırlatıcınızı tamamlamanız gerekmektedir.";

    //Ticket
    public const string TargetUserMustBeTicketCreatorForReply = "Talep formunu yanıtlamak için hedef kullanıcı ticket'ı gönderen kullanıcı olmalıdır.";
    public const string TicketMustNotBeClosed = "Kapanmış bir forma işlem yapamazsınız";
    public const string PerformedByUserAndTargetUserIsSame = "Hedef kullanıcı ve işlemi gerçekleştiren kullanıcı aynı, reply işlemi yapılmalıdır.";
    public const string OnlyCancelbyCreator = "Sadece talep formunu oluşturan kullanıcı iptal edebilir.";
    public const string RequiredTargetUserForTransfer = "Transfer işlemi için hedef kullanıcı bilgisi gereklidir.";

    //General Errors
    public const string TimeLimitExceeded = "İşlem, ilgili zaman kuralı aşıldığı için gerçekleştirilemedi.Yöneticiniz ile iletişime geçebilirsiniz.";
    public const string CompleteRegistration = "Kayıt işlemi başarısız, lütfen terkar deneyiniz.";
    public const string InvalidFileType = "Geçersiz dosya türü.Sadece PDF dosyaları kabul edilmektedir.";
    public const string InvalidFormat = "Geçersiz dosya formatı.";
    public const string InvalidFileContent = "Dosya içeriği geçersiz veya işlenemiyor.";
    public const string PhoneIsNotEmpty = "Telefon numarası boş olamaz.";
    public const string InvalidPhoneNumber = "Geçersiz telefon numarası. Lütfen 8-14 haneli bir telefon numarası giriniz.";
    public const string InvalidReservePhoneNumber = "Yedek Kişi numarası geçersiz. Lütfen 8-14 haneli bir telefon numarası giriniz.";
    public const string LookupNotFound = "Veriler için gerekli olan kayıt bulunamadı.Eşleşme sağlanamdı";
    public const string DuplicatePhoneInFile = "Dosya içinde aynı telefon numarasına sahip birden fazla kayıt bulunmaktadır.";
    public const string InvalidDate = "Geçersiz tarih formatı.";
    public const string InvalidLeadResultCode = "Geçersiz sonuç kodu. Tanımlı bir hiyerarşi değeri olmalıdır.";

    //Authentication Errors
    public const string Unauthorized = "Oturum Bulunamadı. Tekrar Giriş Yapınız.";
    public const string Forbidden = "Yetkisiz Erişim.";
    public const string InvalidCredentials = "E-posta veya şifre hatalı.";
    public const string LockedOut = "Hesap geçici olarak kilitlendi.";
    public const string Problem = "Başarısız işlem! Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";

}
