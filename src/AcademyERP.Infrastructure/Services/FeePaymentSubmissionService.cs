using AcademyERP.Application.DTOs.FeePaymentSubmissions;
using AcademyERP.Application.Interfaces;
using AcademyERP.Domain.Entities.Finance;
using AcademyERP.Domain.Enums;
using AcademyERP.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;

namespace AcademyERP.Infrastructure.Services;

public class FeePaymentSubmissionService : IFeePaymentSubmissionService
{
   private readonly ApplicationDbContext _context;
private readonly IHostEnvironment _environment;

    public FeePaymentSubmissionService(
    ApplicationDbContext context,
    IHostEnvironment environment)
{
    _context = context;
    _environment = environment;
}

    public async Task<List<FeePaymentSubmissionResponse>> GetAllAsync()
    {
        var submissions = await _context.FeePaymentSubmissions
            .AsNoTracking()
            .Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Student)

.Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Course)
            .Include(x => x.Teacher)
            .Include(x => x.PaymentMethod)
            .OrderByDescending(x => x.SubmittedAt)
            .ToListAsync();

        return submissions
            .Select(MapToResponse)
            .ToList();
    }

    public async Task<List<FeePaymentSubmissionResponse>> GetByTeacherAsync(
    Guid applicationUserId)
{
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        return new List<FeePaymentSubmissionResponse>();

    var submissions = await _context.FeePaymentSubmissions
        .AsNoTracking()
        .Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Student)

.Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Course)
        .Include(x => x.Teacher)
        .Include(x => x.PaymentMethod)
        .Where(x => x.TeacherId == teacherId)
        .OrderByDescending(x => x.SubmittedAt)
        .ToListAsync();

    return submissions
        .Select(MapToResponse)
        .ToList();
}

    public async Task<FeePaymentSubmissionResponse?> GetByIdAsync(Guid id)
    {
        var submission = await _context.FeePaymentSubmissions
            .AsNoTracking()
           .Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Student)

.Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Course)
            .Include(x => x.Teacher)
            .Include(x => x.PaymentMethod)
            .FirstOrDefaultAsync(x => x.Id == id);

        return submission == null
            ? null
            : MapToResponse(submission);
    }

    public async Task<FeePaymentSubmissionResponse> SubmitAsync(
    Guid applicationUserId,
    CreateFeePaymentSubmissionRequest request)
{
    if (request.Amount <= 0)
        throw new InvalidOperationException(
            "Payment amount must be greater than zero.");

    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        throw new InvalidOperationException(
            "Teacher profile not found.");

        var invoice = await _context.FeeInvoices
            .Include(x => x.Enrollment)
                .ThenInclude(x => x.Student)
            .FirstOrDefaultAsync(x => x.Id == request.FeeInvoiceId);

        if (invoice == null)
            throw new InvalidOperationException(
                "Fee invoice not found.");


        var paymentMethodExists = await _context.PaymentMethods
            .AnyAsync(x => x.Id == request.PaymentMethodId);

        if (!paymentMethodExists)
            throw new InvalidOperationException(
                "Payment method not found.");

        var pendingAmount = await _context.FeePaymentSubmissions
            .Where(x =>
                x.FeeInvoiceId == request.FeeInvoiceId &&
                x.Status == FeePaymentSubmissionStatus.Pending)
            .SumAsync(x => (decimal?)x.Amount) ?? 0;

        var availableBalance =
            invoice.BalanceAmount - pendingAmount;

        if (request.Amount > availableBalance)
        {
            throw new InvalidOperationException(
                $"Payment amount cannot exceed the available balance of " +
                $"{availableBalance:0.00}.");
        }

        var submission = new FeePaymentSubmission
        {
            FeeInvoiceId = request.FeeInvoiceId,
            TeacherId = teacherId,
            Amount = request.Amount,
            PaymentDate = request.PaymentDate,
            PaymentMethodId = request.PaymentMethodId,
            TransactionReference = request.TransactionReference,
            ProofFilePath = request.ProofFilePath,
            Remarks = request.Remarks,
            Status = FeePaymentSubmissionStatus.Pending,
            SubmittedAt = DateTime.UtcNow
        };

        _context.FeePaymentSubmissions.Add(submission);

        await _context.SaveChangesAsync();

        submission = await _context.FeePaymentSubmissions
            .Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Student)

.Include(x => x.FeeInvoice)
    .ThenInclude(x => x.Enrollment)
        .ThenInclude(x => x.Course)
            .Include(x => x.Teacher)
            .Include(x => x.PaymentMethod)
            .FirstAsync(x => x.Id == submission.Id);

        return MapToResponse(submission);
    }

    public async Task<FeePaymentSubmissionResponse?> UploadProofAsync(
    Guid submissionId,
    Guid applicationUserId,
    Stream fileStream,
    string fileName,
    string contentType)
{
    var teacherId = await _context.Teachers
        .Where(x => x.ApplicationUserId == applicationUserId)
        .Select(x => x.Id)
        .FirstOrDefaultAsync();

    if (teacherId == Guid.Empty)
        throw new InvalidOperationException(
            "Teacher profile not found.");

    var submission = await _context.FeePaymentSubmissions
        .FirstOrDefaultAsync(x =>
            x.Id == submissionId &&
            x.TeacherId == teacherId);

    if (submission == null)
        return null;

    if (submission.Status != FeePaymentSubmissionStatus.Pending)
        throw new InvalidOperationException(
            "Proof can only be uploaded for a pending payment submission.");

    var extension = Path.GetExtension(fileName).ToLowerInvariant();

    var allowedExtensions = new[]
    {
        ".jpg",
        ".jpeg",
        ".png",
        ".webp"
    };

    if (!allowedExtensions.Contains(extension))
        throw new InvalidOperationException(
            "Only JPG, JPEG, PNG and WebP images are allowed.");

    var storageFolder = Path.Combine(
        _environment.ContentRootPath,
        "Storage",
        "PaymentProofs");

    Directory.CreateDirectory(storageFolder);

    var storedFileName =
        $"{submission.Id:N}_{Guid.NewGuid():N}{extension}";

    var fullPath = Path.Combine(
        storageFolder,
        storedFileName);

    await using (var output =
        new FileStream(
            fullPath,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None))
    {
        await fileStream.CopyToAsync(output);
    }

    submission.ProofFilePath =
        Path.Combine(
            "Storage",
            "PaymentProofs",
            storedFileName)
        .Replace("\\", "/");

    submission.PaymentProofFileName =
        Path.GetFileName(fileName);

    submission.PaymentProofUploadedOn =
        DateTime.UtcNow;

    await _context.SaveChangesAsync();

    return await GetByIdAsync(submission.Id);
}

    public async Task<FeePaymentSubmissionResponse?> ReviewAsync(
        Guid id,
        Guid reviewerId,
        ReviewFeePaymentSubmissionRequest request)
    {
        var submission = await _context.FeePaymentSubmissions
            .Include(x => x.FeeInvoice)
            .FirstOrDefaultAsync(x => x.Id == id);

        if (submission == null)
            return null;

        if (submission.Status != FeePaymentSubmissionStatus.Pending)
            throw new InvalidOperationException(
                "This payment submission has already been reviewed.");

        if (!request.Approve &&
            string.IsNullOrWhiteSpace(request.RejectionReason))
        {
            throw new InvalidOperationException(
                "A rejection reason is required when rejecting a payment.");
        }

        if (request.Approve)
        {
            var invoice = submission.FeeInvoice;

            if (submission.Amount > invoice.BalanceAmount)
            {
                throw new InvalidOperationException(
                    "The submitted amount is greater than the current invoice balance.");
            }

            var payment = new FeePayment
            {
                FeeInvoiceId = submission.FeeInvoiceId,
                Amount = submission.Amount,
                PaymentDate = submission.PaymentDate,
                PaymentMethodId = submission.PaymentMethodId,
                TransactionReference = submission.TransactionReference,
                Remarks = submission.Remarks
            };

            _context.FeePayments.Add(payment);

            invoice.PaidAmount += submission.Amount;
            invoice.BalanceAmount =
                invoice.Amount - invoice.PaidAmount;

            if (invoice.PaidAmount <= 0)
            {
                invoice.Status = FeeStatus.Pending;
            }
            else if (invoice.BalanceAmount > 0)
            {
                invoice.Status = FeeStatus.PartiallyPaid;
            }
            else
            {
                invoice.Status = FeeStatus.Paid;
                invoice.PaidDate = submission.PaymentDate;
            }

            submission.Status =
                FeePaymentSubmissionStatus.Approved;

            submission.ReviewedAt = DateTime.UtcNow;
            submission.ReviewedBy = reviewerId;
            submission.RejectionReason = null;
        }
        else
        {
            submission.Status =
                FeePaymentSubmissionStatus.Rejected;

            submission.ReviewedAt = DateTime.UtcNow;
            submission.ReviewedBy = reviewerId;
            submission.RejectionReason =
                request.RejectionReason!.Trim();
        }

        await _context.SaveChangesAsync();

        return await GetByIdAsync(submission.Id);
    }

    private static FeePaymentSubmissionResponse MapToResponse(
        FeePaymentSubmission submission)
    {
        return new FeePaymentSubmissionResponse
        {
            Id = submission.Id,

            FeeInvoiceId = submission.FeeInvoiceId,

            InvoiceNumber =
                submission.FeeInvoice.InvoiceNumber,

            StudentName =
                submission.FeeInvoice
                    .Enrollment
                    .Student
                    .FullName,
                    CourseName = submission.FeeInvoice.Enrollment.Course.CourseName,
Gender = submission.FeeInvoice.Enrollment.Student.Gender.ToString(),

            TeacherId = submission.TeacherId,

            TeacherName =
                submission.Teacher.FullName,

            Amount = submission.Amount,

            PaymentDate = submission.PaymentDate,

            PaymentMethodId =
                submission.PaymentMethodId,

            PaymentMethodName =
                submission.PaymentMethod.Name,

            TransactionReference =
                submission.TransactionReference,

            ProofFilePath =
                submission.ProofFilePath,

                PaymentProofFileName = submission.PaymentProofFileName,

PaymentProofUploadedOn = submission.PaymentProofUploadedOn,

            Remarks =
                submission.Remarks,

            Status =
                submission.Status,

            SubmittedAt =
                submission.SubmittedAt,

            ReviewedAt =
                submission.ReviewedAt,

            ReviewedBy =
                submission.ReviewedBy,

            RejectionReason =
                submission.RejectionReason
        };
    }
}