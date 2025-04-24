
// Type: SQLite.SQLite3
// Assembly: MiBandApp.Storage, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 12BDBED5-65C6-40FC-A182-B057969144FE
// Assembly location: C:\Users\Admin\Desktop\RE\MiBandApp_1.21.4.60\MiBandApp.Storage.dll

using System;
using System.Runtime.InteropServices;
using System.Text;

#nullable disable
namespace SQLite
{
  public static class SQLite3
  {
    [DllImport("sqlite3", EntryPoint = "sqlite3_open", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Open([MarshalAs((UnmanagedType) 20)] string filename, out IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Open(
      [MarshalAs((UnmanagedType) 20)] string filename,
      out IntPtr db,
      int flags,
      IntPtr zvfs);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open_v2", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Open(
      byte[] filename,
      out IntPtr db,
      int flags,
      IntPtr zvfs);

    [DllImport("sqlite3", EntryPoint = "sqlite3_open16", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Open16([MarshalAs((UnmanagedType) 21)] string filename, out IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_enable_load_extension", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result EnableLoadExtension(IntPtr db, int onoff);

    [DllImport("sqlite3", EntryPoint = "sqlite3_close", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Close(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_initialize", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Initialize();

    [DllImport("sqlite3", EntryPoint = "sqlite3_shutdown", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Shutdown();

    [DllImport("sqlite3", EntryPoint = "sqlite3_config", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Config(SQLite3.ConfigOption option);

    [DllImport("sqlite3", EntryPoint = "sqlite3_win32_set_directory", CharSet = CharSet.Unicode, CallingConvention = (CallingConvention) 2)]
    public static extern int SetDirectory(uint directoryType, string directoryPath);

    [DllImport("sqlite3", EntryPoint = "sqlite3_busy_timeout", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result BusyTimeout(IntPtr db, int milliseconds);

    [DllImport("sqlite3", EntryPoint = "sqlite3_changes", CallingConvention = (CallingConvention) 2)]
    public static extern int Changes(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Prepare2(
      IntPtr db,
      [MarshalAs((UnmanagedType) 20)] string sql,
      int numBytes,
      out IntPtr stmt,
      IntPtr pzTail);

    [DllImport("sqlite3", EntryPoint = "sqlite3_prepare_v2", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Prepare2(
      IntPtr db,
      byte[] queryBytes,
      int numBytes,
      out IntPtr stmt,
      IntPtr pzTail);

    public static IntPtr Prepare2(IntPtr db, string query)
    {
      byte[] bytes = Encoding.UTF8.GetBytes(query);
      IntPtr stmt;
      SQLite3.Result r = SQLite3.Prepare2(db, bytes, bytes.Length, out stmt, IntPtr.Zero);
      if (r != SQLite3.Result.OK)
        throw SQLiteException.New(r, SQLite3.GetErrmsg(db));
      return stmt;
    }

    [DllImport("sqlite3", EntryPoint = "sqlite3_step", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Step(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_reset", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Reset(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_finalize", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.Result Finalize(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_last_insert_rowid", CallingConvention = (CallingConvention) 2)]
    public static extern long LastInsertRowid(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_errmsg16", CallingConvention = (CallingConvention) 2)]
    public static extern IntPtr Errmsg(IntPtr db);

    public static string GetErrmsg(IntPtr db) => Marshal.PtrToStringUni(SQLite3.Errmsg(db));

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_parameter_index", CallingConvention = (CallingConvention) 2)]
    public static extern int BindParameterIndex(IntPtr stmt, [MarshalAs((UnmanagedType) 20)] string name);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_null", CallingConvention = (CallingConvention) 2)]
    public static extern int BindNull(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int", CallingConvention = (CallingConvention) 2)]
    public static extern int BindInt(IntPtr stmt, int index, int val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_int64", CallingConvention = (CallingConvention) 2)]
    public static extern int BindInt64(IntPtr stmt, int index, long val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_double", CallingConvention = (CallingConvention) 2)]
    public static extern int BindDouble(IntPtr stmt, int index, double val);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_text16", CharSet = CharSet.Unicode, CallingConvention = (CallingConvention) 2)]
    public static extern int BindText(IntPtr stmt, int index, [MarshalAs((UnmanagedType) 21)] string val, int n, IntPtr free);

    [DllImport("sqlite3", EntryPoint = "sqlite3_bind_blob", CallingConvention = (CallingConvention) 2)]
    public static extern int BindBlob(IntPtr stmt, int index, byte[] val, int n, IntPtr free);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_count", CallingConvention = (CallingConvention) 2)]
    public static extern int ColumnCount(IntPtr stmt);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_name", CallingConvention = (CallingConvention) 2)]
    public static extern IntPtr ColumnName(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_name16", CallingConvention = (CallingConvention) 2)]
    private static extern IntPtr ColumnName16Internal(IntPtr stmt, int index);

    public static string ColumnName16(IntPtr stmt, int index)
    {
      return Marshal.PtrToStringUni(SQLite3.ColumnName16Internal(stmt, index));
    }

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_type", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.ColType ColumnType(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int", CallingConvention = (CallingConvention) 2)]
    public static extern int ColumnInt(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_int64", CallingConvention = (CallingConvention) 2)]
    public static extern long ColumnInt64(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_double", CallingConvention = (CallingConvention) 2)]
    public static extern double ColumnDouble(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_text", CallingConvention = (CallingConvention) 2)]
    public static extern IntPtr ColumnText(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_text16", CallingConvention = (CallingConvention) 2)]
    public static extern IntPtr ColumnText16(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_blob", CallingConvention = (CallingConvention) 2)]
    public static extern IntPtr ColumnBlob(IntPtr stmt, int index);

    [DllImport("sqlite3", EntryPoint = "sqlite3_column_bytes", CallingConvention = (CallingConvention) 2)]
    public static extern int ColumnBytes(IntPtr stmt, int index);

    public static string ColumnString(IntPtr stmt, int index)
    {
      return Marshal.PtrToStringUni(SQLite3.ColumnText16(stmt, index));
    }

    public static byte[] ColumnByteArray(IntPtr stmt, int index)
    {
      int length = SQLite3.ColumnBytes(stmt, index);
      byte[] destination = new byte[length];
      if (length > 0)
        Marshal.Copy(SQLite3.ColumnBlob(stmt, index), destination, 0, length);
      return destination;
    }

    [DllImport("sqlite3", EntryPoint = "sqlite3_extended_errcode", CallingConvention = (CallingConvention) 2)]
    public static extern SQLite3.ExtendedResult ExtendedErrCode(IntPtr db);

    [DllImport("sqlite3", EntryPoint = "sqlite3_libversion_number", CallingConvention = (CallingConvention) 2)]
    public static extern int LibVersionNumber();

    public enum Result
    {
      OK = 0,
      Error = 1,
      Internal = 2,
      Perm = 3,
      Abort = 4,
      Busy = 5,
      Locked = 6,
      NoMem = 7,
      ReadOnly = 8,
      Interrupt = 9,
      IOError = 10, // 0x0000000A
      Corrupt = 11, // 0x0000000B
      NotFound = 12, // 0x0000000C
      Full = 13, // 0x0000000D
      CannotOpen = 14, // 0x0000000E
      LockErr = 15, // 0x0000000F
      Empty = 16, // 0x00000010
      SchemaChngd = 17, // 0x00000011
      TooBig = 18, // 0x00000012
      Constraint = 19, // 0x00000013
      Mismatch = 20, // 0x00000014
      Misuse = 21, // 0x00000015
      NotImplementedLFS = 22, // 0x00000016
      AccessDenied = 23, // 0x00000017
      Format = 24, // 0x00000018
      Range = 25, // 0x00000019
      NonDBFile = 26, // 0x0000001A
      Notice = 27, // 0x0000001B
      Warning = 28, // 0x0000001C
      Row = 100, // 0x00000064
      Done = 101, // 0x00000065
    }

    public enum ExtendedResult
    {
      BusyRecovery = 261, // 0x00000105
      LockedSharedcache = 262, // 0x00000106
      ReadonlyRecovery = 264, // 0x00000108
      IOErrorRead = 266, // 0x0000010A
      CorruptVTab = 267, // 0x0000010B
      CannottOpenNoTempDir = 270, // 0x0000010E
      ConstraintCheck = 275, // 0x00000113
      NoticeRecoverWAL = 283, // 0x0000011B
      AbortRollback = 516, // 0x00000204
      ReadonlyCannotLock = 520, // 0x00000208
      IOErrorShortRead = 522, // 0x0000020A
      CannotOpenIsDir = 526, // 0x0000020E
      ConstraintCommitHook = 531, // 0x00000213
      NoticeRecoverRollback = 539, // 0x0000021B
      ReadonlyRollback = 776, // 0x00000308
      IOErrorWrite = 778, // 0x0000030A
      CannotOpenFullPath = 782, // 0x0000030E
      ConstraintForeignKey = 787, // 0x00000313
      IOErrorFsync = 1034, // 0x0000040A
      ConstraintFunction = 1043, // 0x00000413
      IOErrorDirFSync = 1290, // 0x0000050A
      ConstraintNotNull = 1299, // 0x00000513
      IOErrorTruncate = 1546, // 0x0000060A
      ConstraintPrimaryKey = 1555, // 0x00000613
      IOErrorFStat = 1802, // 0x0000070A
      ConstraintTrigger = 1811, // 0x00000713
      IOErrorUnlock = 2058, // 0x0000080A
      ConstraintUnique = 2067, // 0x00000813
      IOErrorRdlock = 2314, // 0x0000090A
      ConstraintVTab = 2323, // 0x00000913
      IOErrorDelete = 2570, // 0x00000A0A
      IOErrorBlocked = 2826, // 0x00000B0A
      IOErrorNoMem = 3082, // 0x00000C0A
      IOErrorAccess = 3338, // 0x00000D0A
      IOErrorCheckReservedLock = 3594, // 0x00000E0A
      IOErrorLock = 3850, // 0x00000F0A
      IOErrorClose = 4106, // 0x0000100A
      IOErrorDirClose = 4362, // 0x0000110A
      IOErrorSHMOpen = 4618, // 0x0000120A
      IOErrorSHMSize = 4874, // 0x0000130A
      IOErrorSHMLock = 5130, // 0x0000140A
      IOErrorSHMMap = 5386, // 0x0000150A
      IOErrorSeek = 5642, // 0x0000160A
      IOErrorDeleteNoEnt = 5898, // 0x0000170A
      IOErrorMMap = 6154, // 0x0000180A
    }

    public enum ConfigOption
    {
      SingleThread = 1,
      MultiThread = 2,
      Serialized = 3,
    }

    public enum ColType
    {
      Integer = 1,
      Float = 2,
      Text = 3,
      Blob = 4,
      Null = 5,
    }
  }
}
