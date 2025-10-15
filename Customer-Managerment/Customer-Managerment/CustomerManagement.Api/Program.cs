using Customer_Managerment.CustomerManagement.Api.GraphQL;
using Customer_Managerment.CustomerManagement.Application.Interfaces;
using Customer_Managerment.CustomerManagement.Application.UseCases.Company;
using Customer_Managerment.CustomerManagement.Infrastructure.Data;
using Customer_Managerment.CustomerManagement.Infrastructure.Repositories;
using HotChocolate.AspNetCore.Voyager;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);


DotNetEnv.Env.Load();

builder.Configuration["ConnectionStrings:PostgreSQLConnection"] = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__PostgreSQLConnection");

// DbContext Registration
builder.Services.AddPooledDbContextFactory<CustomerManagementDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSQLConnection"))
);


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Repositories
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();



// Handlers
builder.Services.AddScoped<CompanyHandler>();




// Mapper Registration
builder.Services.AddAutoMapper(typeof(Program));

// GraphQL Registration

builder.Services
    .AddGraphQLServer()
    .AddQueryType<Query>()
    .AddMutationType<Mutation>()
    .AddFiltering()
    .AddSorting()
    .AddProjections();


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

// Endpoint GraphQL
app.MapGraphQL("/graphql");

// Voyager UI để trực quan hóa schema
app.UseVoyager(
    "/graphql",  // endpoint của GraphQL
    "/voyager"   // đường dẫn để mở giao diện Voyager
);

app.MapControllers();

app.Run();

