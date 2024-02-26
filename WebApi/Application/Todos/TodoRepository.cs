using WebApi.Application.Abstractions;
using WebApi.Infrastructure.Database;
using WebApi.Models;

namespace WebApi.Application.Todos;

public class TodoRepository(AppDbContext context) : Repository<Todo>(context) { }
