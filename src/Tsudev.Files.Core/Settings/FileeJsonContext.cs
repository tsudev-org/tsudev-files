// System.Text.Json source generation for everything Filee persists. Reflection-free and trim-safe.

using System.Text.Json.Serialization;
using Tsudev.Files.Core.History;
using Tsudev.Files.Core.Presets;
using Tsudev.Files.Core.Profiles;

namespace Tsudev.Files.Core.Settings;

[JsonSourceGenerationOptions(
    WriteIndented = true,
    PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase,
    UseStringEnumConverter = true,
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    ReadCommentHandling = System.Text.Json.JsonCommentHandling.Skip,
    AllowTrailingCommas = true)]
[JsonSerializable(typeof(AppSettings))]
[JsonSerializable(typeof(ThemeStyle))]
[JsonSerializable(typeof(List<Preset>))]
[JsonSerializable(typeof(TranslateOptions))]
[JsonSerializable(typeof(List<ToolbarProfile>))]
[JsonSerializable(typeof(PresetBundle))]
[JsonSerializable(typeof(List<HistoryEntry>))]
[JsonSerializable(typeof(CustomTheme))]
internal sealed partial class TsudevFilesJsonContext : JsonSerializerContext;

/// <summary>File format used to import / export presets and profiles.</summary>
public sealed class PresetBundle
{
    public int SchemaVersion { get; set; } = 1;
    public List<Preset> Presets { get; set; } = [];
    public List<ToolbarProfile> Profiles { get; set; } = [];
}
