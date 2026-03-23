namespace Corelink.Domain.Entities;

public sealed class SupabaseOptions
{
    public const string SectionName = "Supabase";

    public string Url { get; init; } = null!;
    public string Servicekey { get; init; } = null!;
    public string Bucket { get; init; } = null!;
}