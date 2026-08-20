namespace AcademyERP.Admin.Models.StudentReports;



public class StudentReportDashboardResponse
{
    public int TotalStudents { get; set; }

    public int ActiveStudents { get; set; }

    public decimal AveragePerformance { get; set; }

    public decimal AverageAttendance { get; set; }


    public BestStudentDto? BestStudent { get; set; }


    public List<StudentPerformanceTrendDto> PerformanceTrend { get; set; }
        = new();


    public List<StudentAttendanceTrendDto> AttendanceTrend { get; set; }
        = new();


    public List<CourseStudentDistributionDto> CourseDistribution { get; set; }
        = new();


    public List<StudentRankingDto> StudentRanking { get; set; }
        = new();


    public List<StudentStatusDto> StudentStatus { get; set; }
        = new();

        public List<StudentLearningProgressDto> LearningProgress { get; set; }
= new();

public List<StudentFeeStatusDto> FeeStatus { get; set; }
= new();
public AttendanceSummaryDashboardDto AttendanceSummary { get; set; }
= new();

public StudentFeeSummaryDto FeeSummary { get; set; }
= new();

public StudentLearningSummaryDto LearningSummary { get; set; }
= new();
}