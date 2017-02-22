using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace Sasila.Common.Tools
{
    [Serializable]
    public struct HardDiskInfo
    {
        /// <summary>
        /// 型号
        /// </summary>
        public string ModuleNumber;
        /// <summary>
        /// 固件版本
        /// </summary>
        public string Firmware;
        /// <summary>
        /// 序列号
        /// </summary>
        public string SerialNumber;
        /// <summary>
        /// 容量，以M为单位
        /// </summary>
        public uint Capacity;
    }

    #region Internal Structs

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct GetVersionOutParams
    {
        public byte bVersion;
        public byte bRevision;
        public byte bReserved;
        public byte bIDEDeviceMap;
        public uint fCapabilities;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dwReserved; // For future use.
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct IdeRegs
    {
        public byte bFeaturesReg;
        public byte bSectorCountReg;
        public byte bSectorNumberReg;
        public byte bCylLowReg;
        public byte bCylHighReg;
        public byte bDriveHeadReg;
        public byte bCommandReg;
        public byte bReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SendCmdInParams
    {
        public uint cBufferSize;
        public IdeRegs irDriveRegs;
        public byte bDriveNumber;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] bReserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] dwReserved;
        public byte bBuffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct DriverStatus
    {
        public byte bDriverError;
        public byte bIDEStatus;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] bReserved;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] dwReserved;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SendCmdOutParams
    {
        public uint cBufferSize;
        public DriverStatus DriverStatus;
        public IdSector bBuffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1, Size = 512)]
    internal struct IdSector
    {
        public ushort wGenConfig;
        public ushort wNumCyls;
        public ushort wReserved;
        public ushort wNumHeads;
        public ushort wBytesPerTrack;
        public ushort wBytesPerSector;
        public ushort wSectorsPerTrack;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public ushort[] wVendorUnique;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 20)]
        public byte[] sSerialNumber;
        public ushort wBufferType;
        public ushort wBufferSize;
        public ushort wECCSize;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] sFirmwareRev;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 40)]
        public byte[] sModelNumber;
        public ushort wMoreVendorUnique;
        public ushort wDoubleWordIO;
        public ushort wCapabilities;
        public ushort wReserved1;
        public ushort wPIOTiming;
        public ushort wDMATiming;
        public ushort wBS;
        public ushort wNumCurrentCyls;
        public ushort wNumCurrentHeads;
        public ushort wNumCurrentSectorsPerTrack;
        public uint ulCurrentSectorCapacity;
        public ushort wMultSectorStuff;
        public uint ulTotalAddressableSectors;
        public ushort wSingleWordDMA;
        public ushort wMultiWordDMA;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 128)]
        public byte[] bReserved;
    }

    internal enum STORAGE_PROPERTY_ID
    {
        StorageDeviceProperty = 0,
        StorageAdapterProperty,
        StorageDeviceIdProperty,
        StorageDeviceUniqueIdProperty,
        StorageDeviceWriteCacheProperty,
        StorageMiniportProperty,
        StorageAccessAlignmentProperty
    }
    internal enum STORAGE_QUERY_TYPE
    {
        PropertyStandardQuery = 0,          // Retrieves the descriptor
        PropertyExistsQuery,                // Used to test whether the descriptor is supported
        PropertyMaskQuery,                  // Used to retrieve a mask of writeable fields in the descriptor
        PropertyQueryMaxDefined     // use to validate the value
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct STORAGE_PROPERTY_QUERY
    {
        public STORAGE_PROPERTY_ID PropertyId;
        public STORAGE_QUERY_TYPE QueryType;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] AdditionalParameters;
    }


    internal enum STORAGE_BUS_TYPE
    {
        BusTypeUnknown = 0x00,
        BusTypeScsi,
        BusTypeAtapi,
        BusTypeAta,
        BusType1394,
        BusTypeSsa,
        BusTypeFibre,
        BusTypeUsb,
        BusTypeRAID,
        BusTypeiScsi,
        BusTypeSas,
        BusTypeSata,
        BusTypeSd,
        BusTypeMmc,
        BusTypeMax,
        BusTypeMaxReserved = 0x7F
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct STORAGE_DEVICE_DESCRIPTOR
    {
        public UInt32 Version;
        public UInt32 Size;
        public byte DeviceType;
        public byte DeviceTypeModifier;
        public byte RemovableMedia;
        public byte CommandQueueing;
        public UInt32 VendorIdOffset;
        public UInt32 ProductIdOffset;
        public UInt32 ProductRevisionOffset;
        public UInt32 SerialNumberOffset;
        public STORAGE_BUS_TYPE BusType;
        public UInt32 RawPropertiesLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1)]
        public byte[] RawDeviceProperties;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 1000)]
        public byte[] buffer;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct SRB_IO_CONTROL
    {
        public UInt32 HeaderLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] Signature;
        public UInt32 Timeout;
        public UInt32 ControlCode;
        public UInt32 ReturnCode;
        public UInt32 Length;

        public SendCmdInParams pin;
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    internal struct OUT_BUFFER
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x1c)]
        public byte[] sic;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 0x10)]
        public byte[] pout;
        public IdSector ids;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 512)]
        public byte[] buffer;
    }
    #endregion

    /// <summary>
    /// ATAPI驱动器相关
    /// </summary>
    public class DeviceID
    {
        #region DllImport
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern int CloseHandle(IntPtr hObject);

        [DllImport("kernel32.dll", SetLastError = true)]
        static extern IntPtr CreateFile(string lpFileName, uint dwDesiredAccess, uint dwShareMode, IntPtr lpSecurityAttributes, uint dwCreationDisposition, uint dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, IntPtr lpInBuffer, uint nInBufferSize, ref GetVersionOutParams lpOutBuffer, uint nOutBufferSize, ref uint lPBytesReturned, [Out] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, ref SendCmdInParams lpInBuffer, uint nInBufferSize, ref SendCmdOutParams lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, [Out] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, ref STORAGE_PROPERTY_QUERY lpInBuffer, uint nInBufferSize, ref STORAGE_DEVICE_DESCRIPTOR lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, [Out] IntPtr lpOverlapped);

        [DllImport("kernel32.dll")]
        static extern int DeviceIoControl(IntPtr hDevice, uint dwIoControlCode, ref SRB_IO_CONTROL lpInBuffer, uint nInBufferSize, ref OUT_BUFFER lpOutBuffer, uint nOutBufferSize, ref uint lpBytesReturned, [Out] IntPtr lpOverlapped);

        const uint DFP_GET_VERSION = 0x00074080;
        const uint DFP_SEND_DRIVE_COMMAND = 0x0007c084;
        const uint DFP_RECEIVE_DRIVE_DATA = 0x0007c088;

        const uint SMART_GET_VERSION = 0x00074080;
        const uint SMART_RCV_DRIVE_DATA = 0x0007c088;

        const uint IOCTL_STORAGE_QUERY_PROPERTY = 0x002d1400;
        const uint IOCTL_SCSI_MINIPORT_IDENTIFY = 0x1b0501;
        const uint IOCTL_SCSI_MINIPORT = 0x0004D008;
        const byte IDE_ATA_IDENTIFY = 0xec;

        const uint GENERIC_READ = 0x80000000;
        const uint GENERIC_WRITE = 0x40000000;
        const uint FILE_SHARE_READ = 0x00000001;
        const uint FILE_SHARE_WRITE = 0x00000002;
        const uint CREATE_NEW = 1;
        const uint OPEN_EXISTING = 3;

        const byte ID_CMD = 0xec;

        #endregion

        #region GetHddInfo

        public static string GetHddInfo(byte driveIndex)
        {
            switch (Environment.OSVersion.Platform)
            {
                case PlatformID.Win32Windows:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.Win32NT:
                    return GetHddInfoAsScsiDriveInNT(driveIndex);
                case PlatformID.Win32S:
                    throw new NotSupportedException("Win32s is not supported.");
                case PlatformID.WinCE:
                    throw new NotSupportedException("WinCE is not supported.");
                default:
                    throw new NotSupportedException("Unknown Platform.");
            }
        }

        #region GetHddInfoNT
        /// <summary>
        /// ReadPhysicalDriveInNTWithAdminRights
        /// </summary>
        /// <param name="driveIndex">驱动编号</param>
        /// <returns></returns>
        public static string GetHddInfoNTWithAdminRights(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            // We start in NT/Win2000
            IntPtr hDevice = CreateFile(string.Format(@"\\.\PhysicalDrive{0}", driveIndex), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
                return "";

            if (DeviceIoControl(hDevice, DFP_GET_VERSION, IntPtr.Zero, 0, ref vers, (uint)Marshal.SizeOf(vers), ref bytesReturned, IntPtr.Zero) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }
            // If IDE identify command not supported, fails
            if ((vers.fCapabilities & 1) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }
            // Identify the IDE drives
            if ((driveIndex & 1) != 0)
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xb0;
            }
            else
            {
                inParam.irDriveRegs.bDriveHeadReg = 0xa0;
            }
            if ((vers.fCapabilities & (16 >> driveIndex)) != 0)
            {
                // We don''t detect a ATAPI device.
                CloseHandle(hDevice);
                return "";
            }
            else
            {
                inParam.irDriveRegs.bCommandReg = 0xec;
            }
            inParam.bDriveNumber = driveIndex;
            inParam.irDriveRegs.bSectorCountReg = 1;
            inParam.irDriveRegs.bSectorNumberReg = 1;
            inParam.cBufferSize = 512;

            if (DeviceIoControl(hDevice, DFP_RECEIVE_DRIVE_DATA, ref inParam, (uint)Marshal.SizeOf(inParam), ref outParam, (uint)Marshal.SizeOf(outParam), ref bytesReturned, IntPtr.Zero) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }
            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer).SerialNumber;
        }
        //ReadPhysicalDriveInNTUsingSmart
        public static string GetHddInfoNTUsingSmart(byte driveIndex)
        {
            GetVersionOutParams vers = new GetVersionOutParams();
            SendCmdInParams inParam = new SendCmdInParams();
            SendCmdOutParams outParam = new SendCmdOutParams();
            uint bytesReturned = 0;

            IntPtr hDevice = CreateFile(string.Format(@"\\.\PhysicalDrive{0}", driveIndex), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
                return "";

            if (DeviceIoControl(hDevice, SMART_GET_VERSION, IntPtr.Zero, 0, ref vers, (uint)Marshal.SizeOf(vers), ref bytesReturned, IntPtr.Zero) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }

            inParam.irDriveRegs.bCommandReg = ID_CMD;
            if (DeviceIoControl(hDevice, SMART_RCV_DRIVE_DATA, ref inParam, (uint)Marshal.SizeOf(inParam), ref outParam, (uint)Marshal.SizeOf(outParam), ref bytesReturned, IntPtr.Zero) == 0)
                return "";

            CloseHandle(hDevice);

            return GetHardDiskInfo(outParam.bBuffer).SerialNumber;
        }
        //ReadPhysicalDriveInNTWithZeroRights
        public static string GetHddInfoNTWithZeroRights(byte driveIndex)
        {
            IntPtr hDevice = CreateFile(string.Format(@"\\.\PhysicalDrive{0}", driveIndex), 0, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
                return "";

            STORAGE_PROPERTY_QUERY query = new STORAGE_PROPERTY_QUERY();
            query.PropertyId = STORAGE_PROPERTY_ID.StorageDeviceProperty;
            query.QueryType = STORAGE_QUERY_TYPE.PropertyStandardQuery;

            uint bytesReturned = 0;
            STORAGE_DEVICE_DESCRIPTOR descrip = new STORAGE_DEVICE_DESCRIPTOR();
            if (DeviceIoControl(hDevice, IOCTL_STORAGE_QUERY_PROPERTY, ref query, (uint)Marshal.SizeOf(query), ref descrip, (uint)Marshal.SizeOf(descrip), ref bytesReturned, IntPtr.Zero) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }
            CloseHandle(hDevice);

            uint offset = descrip.SerialNumberOffset - 0x25;
            int len = 0;
            for (int i = 0; i < descrip.buffer.Length; i++)
            {
                if (descrip.buffer[offset + i] == 0)
                    break;
                len++;
            }
            byte[] temp = new byte[len];
            for (int i = 0; i < len; i++)
            {
                temp[i] = descrip.buffer[offset + i];
            }

            System.Text.ASCIIEncoding converter = new System.Text.ASCIIEncoding();
            string aa = converter.GetString(temp);
            //byte[] mc = new byte[aa.Length/2];
            //for (int i = 0; i < aa.Length / 2; i++)
            //    mc[i] = Convert.ToByte(aa.Substring(i * 2, 2), 16);

            ChangeByteOrder(temp);

            string id = converter.GetString(temp);

            id.Replace(" ", "");

            return id;
        }
        //ReadIdeDriveAsScsiDriveInNT
        public static string GetHddInfoAsScsiDriveInNT(byte driveIndex)
        {
            IntPtr hDevice = CreateFile(string.Format(@"\\.\Scsi{0}:", driveIndex), GENERIC_READ | GENERIC_WRITE, FILE_SHARE_READ | FILE_SHARE_WRITE, IntPtr.Zero, OPEN_EXISTING, 0, IntPtr.Zero);
            if (hDevice == IntPtr.Zero)
                return "";


            SRB_IO_CONTROL sic = new SRB_IO_CONTROL();
            sic.HeaderLength = 0x1c;
            sic.Timeout = 10000;
            sic.Length = 0x211;
            sic.ControlCode = IOCTL_SCSI_MINIPORT_IDENTIFY;
            sic.Signature = new byte[8];
            sic.Signature[0] = (byte)'S';
            sic.Signature[1] = (byte)'C';
            sic.Signature[2] = (byte)'S';
            sic.Signature[3] = (byte)'I';
            sic.Signature[4] = (byte)'D';
            sic.Signature[5] = (byte)'I';
            sic.Signature[6] = (byte)'S';
            sic.Signature[7] = (byte)'K';

            sic.pin.irDriveRegs.bCommandReg = IDE_ATA_IDENTIFY;
            sic.pin.bDriveNumber = driveIndex;

            OUT_BUFFER ob = new OUT_BUFFER();

            uint bytesReturned = 0;
            if (DeviceIoControl(hDevice, IOCTL_SCSI_MINIPORT, ref sic, 0x3c, ref ob, (uint)Marshal.SizeOf(ob), ref bytesReturned, IntPtr.Zero) == 0)
            {
                CloseHandle(hDevice);
                return "";
            }

            IntPtr pnt = Marshal.AllocHGlobal(1000);


            CloseHandle(hDevice);

            return (GetHardDiskInfo(ob.ids)).SerialNumber;
        }


        #endregion

        private static HardDiskInfo GetHardDiskInfo(IdSector phdinfo)
        {
            HardDiskInfo hddInfo = new HardDiskInfo();

            ChangeByteOrder(phdinfo.sModelNumber);
            hddInfo.ModuleNumber = Encoding.ASCII.GetString(phdinfo.sModelNumber).Trim();

            ChangeByteOrder(phdinfo.sFirmwareRev);
            hddInfo.Firmware = Encoding.ASCII.GetString(phdinfo.sFirmwareRev).Trim();

            ChangeByteOrder(phdinfo.sSerialNumber);
            hddInfo.SerialNumber = Encoding.ASCII.GetString(phdinfo.sSerialNumber).Trim();

            hddInfo.Capacity = phdinfo.ulTotalAddressableSectors / 2 / 1024;

            return hddInfo;
        }

        private static void ChangeByteOrder(byte[] charArray)
        {
            byte temp;
            for (int i = 0; i < charArray.Length; i += 2)
            {
                temp = charArray[i];
                charArray[i] = charArray[i + 1];
                charArray[i + 1] = temp;
            }
        }
        #endregion
    }
}
