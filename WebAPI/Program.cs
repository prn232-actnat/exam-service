using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Repositories.Data;
using Repositories.Implementations;
using Repositories.Interface;
using Services.Implementations;
using Services.Interface;
using Services.Mappings;
using WebAPI.Filters;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ExamServiceDbContext>(options =>
    options.UseSqlServer(connectionString,
        // Chỉ định cho EF Core biết file Migrations nằm ở project nào
        b => b.MigrationsAssembly("Repositories")));

// --- 2. Đăng ký AutoMapper (cho Lớp Services) ---
// Tự động tìm tất cả các Profile (như AutoMapperProfile)
// trong Assembly (project) của Lớp Services
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);


builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

// Đăng ký Lớp Services (BLL)
builder.Services.AddScoped<IExamService, ExamService>();
builder.Services.AddScoped<IQuestionBankService, QuestionBankService>();
builder.Services.AddScoped<ISubmissionService, SubmissionService>();
builder.Services.AddScoped<IExamSetService, ExamSetService>();

builder.Services.AddControllers(options =>
{
    // Đăng ký Global Exception Filter
    options.Filters.Add<ApiExceptionFilterAttribute>();
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
