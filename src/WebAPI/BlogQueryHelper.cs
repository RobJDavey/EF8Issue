using Microsoft.EntityFrameworkCore;

namespace WebAPI;

public class BlogQueryHelper
{
    public static readonly Func<BlogContext, int, IEnumerable<Blog>> CompiledStaticReadonly =
        EF.CompileQuery((BlogContext ctx, int id) => ctx.Blogs.Where(b => b.Id == id));

    public readonly Func<BlogContext, int, IEnumerable<Blog>> CompiledReadonly =
        EF.CompileQuery((BlogContext ctx, int id) => ctx.Blogs.Where(b => b.Id == id));
}
