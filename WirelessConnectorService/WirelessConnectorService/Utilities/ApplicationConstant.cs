namespace WirelessConnectorService.Utilities
{
    public static class ApplicationConstant
    {
        public const string DateTimeFormat = "yyyy-MM-dd HH:mm:ss";

        public const string SensorLiveDataTableName = "SensorLiveData";

        public const int SqlBulkCopyBatchSize = 100;

        public const int ServiceBusQueueMessageSize = 100;

        public const int ServiceBusQueueReadTimeoutSecond = 60;

        public const string FirebaseApplicationConfiguration = "Firebase";

        public const string DeviceAlertHeading = "Device Alert";

        public const string NotificationAuthorizationEntryKey = "ApiKey";

        public const string NotificationSenderEntryKey = "NotificationSender";

        public const string NotificationReceiverEntryKey = "NotificationReceiver";
    }
}
