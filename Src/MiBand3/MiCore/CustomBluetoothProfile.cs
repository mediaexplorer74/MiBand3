// This code defines a class structure for a custom Bluetooth profile, including various services and characteristics.
using System;

namespace MiBand3
{
    public class CustomBluetoothProfile
    {
        public class Generic
        {
            public static Guid Service = new Guid("00001800-0000-1000-8000-00805F9B34FB");
            public static Guid DeviceNameCharacteristic = new Guid("00002A00-0000-1000-8000-00805F9B34FB");
        }

        public class Information
        {
            public static Guid Service = new Guid("0000180A-0000-1000-8000-00805F9B34FB");
            public static Guid SoftwareCharacteristic = new Guid("00002A28-0000-1000-8000-00805F9B34FB");
        }

        public class Basic
        {
            public static Guid Service = new Guid("0000fee0-0000-1000-8000-00805f9b34fb");
            public static Guid TimeCharacteristic = new Guid("00002A2B-0000-1000-8000-00805F9B34FB");
            public static Guid ConfigurationCharacteristic = new Guid("00000003-0000-3512-2118-0009AF100700");
            public static Guid UserSettingsCharacteristic = new Guid("00000008-0000-3512-2118-0009AF100700");

            private const byte EndpointDisplay = 6;

            public static byte[] CommandEnableDisplayOnLiftWrist = { EndpointDisplay, 5, 0, 1 };
            public static byte[] CommandDisableDisplayOnLiftWrist = { EndpointDisplay, 5, 0, 0 };
            public static byte[] WearLocationLeftWrist = { 32, 0, 0, 2 };
            public static byte[] WearLocationRightWrist = { 32, 0, 0, 130 };

            public static byte CommandSetUserInfo = 79;

            public static byte[] DateFormatDateTime = { EndpointDisplay, 10, 0, 3 };
            public static byte[] DateFormatTime = { EndpointDisplay, 10, 0, 0 };
            public static byte[] DateFormatTime12Hours = { EndpointDisplay, 2, 0, 0 };
            public static byte[] DateFormatTime24Hours = { EndpointDisplay, 2, 0, 1 };
            public static byte[] CommandEnableGoalNotification = { EndpointDisplay, 6, 0, 1 };
            public static byte[] CommandDisableGoalNotification = { EndpointDisplay, 6, 0, 0 };
            public static byte[] CommandEnableRotateWristToSwitchInfo = { EndpointDisplay, 13, 0, 1 };
            public static byte[] CommandDisableRotateWristToSwitchInfo = { EndpointDisplay, 13, 0, 0 };
            public static byte[] CommandEnableDisplayCaller = { EndpointDisplay, 16, 0, 0, 1 };
            public static byte[] CommandDisableDisplayCaller = { EndpointDisplay, 16, 0, 0, 0 };
            public static byte[] CommandDistanceUnitMetric = { EndpointDisplay, 3, 0, 0 };
            public static byte[] CommandDistanceUnitImperial = { EndpointDisplay, 3, 0, 1 };

            public static byte EndpointDnd = 9;
            public static byte[] CommandDoNotDisturbAutomatic = { EndpointDnd, 131 };
            public static byte[] CommandDoNotDisturbOff = { EndpointDnd, 130 };
            public static byte[] CommandDoNotDisturbScheduled = { EndpointDnd, 129, 1, 0, 6, 0 };

            public static byte DndByteStartHours = 2;
            public static byte DndByteStartMinutes = 3;
            public static byte DndByteEndHours = 4;
            public static byte DndByteEndMinutes = 5;

            public static byte EndpointDisplayItems = 10;

            public static byte DisplayItemBitClock = 1;
            public static byte DisplayItemBitSteps = 2;
            public static byte DisplayItemBitDistance = 4;
            public static byte DisplayItemBitCalories = 8;
            public static byte DisplayItemBitHeartRate = 16;
            public static byte DisplayItemBitBattery = 32;

            public static byte ScreenChangeByte = 1;
            public static byte[] CommandChangeScreens = { EndpointDisplayItems, DisplayItemBitClock, 0, 0, 1, 2, 3, 4, 5 };
        }

        public class AlertLevel
        {
            public static Guid Service = new Guid("00001802-0000-1000-8000-00805f9b34fb");
            public static Guid AlertLevelCharacteristic = new Guid("00002a06-0000-1000-8000-00805f9b34fb");
        }

        public class NotificationService
        {
            public static Guid Service = new Guid("00001811-0000-1000-8000-00805F9B34FB");
            public static Guid NewAlertCharacteristic = new Guid("00002A46-0000-1000-8000-00805F9B34FB");

            public const int AlertLevelNone = 0;
            public const int AlertLevelMessage = 1;
            public const int AlertLevelPhoneCall = 2;
            public const int AlertLevelVibrateOnly = 3;
            public const int AlertLevelCustom = 250; // HEX-Value 0xfa

            public const int IconChat = 0;
            public const int IconPenguin = 1;
            public const int IconChatMi = 2;
            public const int IconFb = 3;
            public const int IconTwitter = 4;
            public const int IconMiBand = 5;
            public const int IconSnapchat = 6;
            public const int IconWhatsApp = 7;
            public const int IconManta = 8;
            public const int IconXx0 = 9;
            public const int IconAlarm = 16;
            public const int IconShatteredGlass = 17;
            public const int IconInstagram = 18;
            public const int IconChatGhost = 19;
            public const int IconCow = 20;
            public const int IconXx2 = 21;
            public const int IconXx3 = 22;
            public const int IconXx4 = 23;
            public const int IconXx5 = 24;
            public const int IconXx6 = 25;
            public const int IconEagle = 26;
            public const int IconCalendar = 27;
            public const int IconXx7 = 28;
            public const int IconPhoneCall = 29;
            public const int IconChatLine = 30;
            public const int IconTelegram = 31;
            public const int IconChatTalk = 32;
            public const int IconSkype = 33;
            public const int IconVk = 34;
            public const int IconCircles = 35;
            public const int IconHangouts = 36;
            public const int IconMi = 37;
        }

        public class HeartRate
        {
            public static Guid Service = new Guid("0000180d-0000-1000-8000-00805f9b34fb");
            public static Guid MeasurementCharacteristic = new Guid("00002a37-0000-1000-8000-00805f9b34fb");
            public static Guid Descriptor = new Guid("00002902-0000-1000-8000-00805f9b34fb");
            public static Guid ControlCharacteristic = new Guid("00002a39-0000-1000-8000-00805f9b34fb");
        }

        public class Authentication
        {
            public static Guid Service = new Guid("0000FEE1-0000-1000-8000-00805F9B34FB");
            public static Guid AuthCharacteristic = new Guid("00000009-0000-3512-2118-0009af100700");

            public const int AuthSendKey = 1; // HEX Value 0x01
            public const int AuthRequestRandomAuthNumber = 2; // HEX Value 0x02
            public const int AuthSendEncryptedAuthNumber = 3; // HEX Value 0x03
            public const int AuthResponse = 16; // HEX Value 0x10
            public const int AuthSuccess = 1; // HEX Value 0x01
            public const int AuthFail = 4; // HEX Value 0x04
            public const int AuthByte = 8; // HEX Value 0x08
        }
    }
}

