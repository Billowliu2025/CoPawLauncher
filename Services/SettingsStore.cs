using System.Diagnostics;
using System.IO;
using Microsoft.Data.Sqlite;

namespace CoPawLauncher.Services;

/// <summary>
/// 基于 SQLite 的设置持久化存储
/// 数据库文件位于 %LOCALAPPDATA%\CoPawLauncher\settings.db
/// </summary>
public static class SettingsStore
{
    private static readonly string _dbFolder =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "CoPawLauncher");

    private static readonly string _dbPath = Path.Combine(_dbFolder, "settings.db");

    private static string ConnectionString => $"Data Source={_dbPath}";

    /// <summary>
    /// 初始化数据库，创建表（如果不存在）
    /// </summary>
    public static void Initialize()
    {
        try
        {
            Directory.CreateDirectory(_dbFolder);

            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                CREATE TABLE IF NOT EXISTS Settings (
                    Key   TEXT PRIMARY KEY,
                    Value TEXT NOT NULL
                );
                """;
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"初始化设置数据库失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 读取设置值
    /// </summary>
    /// <param name="key">设置键</param>
    /// <param name="defaultValue">默认值</param>
    /// <returns>设置值，读取失败时返回默认值</returns>
    public static string Get(string key, string defaultValue = "")
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = "SELECT Value FROM Settings WHERE Key = @key";
            command.Parameters.AddWithValue("@key", key);

            var result = command.ExecuteScalar();
            return result?.ToString() ?? defaultValue;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"读取设置 [{key}] 失败：{ex.Message}");
            return defaultValue;
        }
    }

    /// <summary>
    /// 保存设置值（存在则更新，不存在则插入）
    /// </summary>
    /// <param name="key">设置键</param>
    /// <param name="value">设置值</param>
    public static void Set(string key, string value)
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();

            var command = connection.CreateCommand();
            command.CommandText = """
                INSERT INTO Settings (Key, Value) VALUES (@key, @value)
                ON CONFLICT(Key) DO UPDATE SET Value = @value;
                """;
            command.Parameters.AddWithValue("@key", key);
            command.Parameters.AddWithValue("@value", value);
            command.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"保存设置 [{key}={value}] 失败：{ex.Message}");
        }
    }

    /// <summary>
    /// 读取布尔设置
    /// </summary>
    public static bool GetBool(string key, bool defaultValue = false)
    {
        var value = Get(key);
        return string.IsNullOrEmpty(value) ? defaultValue : value == "1";
    }

    /// <summary>
    /// 保存布尔设置
    /// </summary>
    public static void SetBool(string key, bool value)
    {
        Set(key, value ? "1" : "0");
    }

    #region 设置键常量

    /// <summary>是否为深色主题</summary>
    public const string KeyIsDarkTheme = "IsDarkTheme";

    /// <summary>主色调名称</summary>
    public const string KeyPrimaryColor = "PrimaryColor";

    /// <summary>自定义图标文件名</summary>
    public const string KeyCustomIconFile = "CustomIconFile";

    /// <summary>
    /// 获取持久化存储目录（%LOCALAPPDATA%\CoPawLauncher\)
    /// </summary>
    public static string DataFolder => _dbFolder;

    #endregion
}
