namespace AcademyERP.Admin.Models.Registration;

public class StudentApprovalModel
{
    public Guid RegistrationStudentId
    {
        get;
        set;
    }

    public Guid? CourseId
    {
        get;
        set;
    }

    public Guid? TeacherId
    {
        get;
        set;
    }

    public Guid? TimeSlotId
    {
        get;
        set;
    }

    public Guid? ClassDurationId
    {
        get;
        set;
    }

    public List<DayOfWeek> DaysOfWeek
    {
        get;
        set;
    } = new();

    // Finance
    public decimal StandardCourseFee
    {
        get;
        set;
    }

    public decimal FinalStudentFee
    {
        get;
        set;
    }

    public decimal TeacherPaymentAmount
    {
        get;
        set;
    }
}
