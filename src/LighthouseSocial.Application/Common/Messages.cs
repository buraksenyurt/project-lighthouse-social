namespace LighthouseSocial.Application.Common;

public static class Messages
{
    public static class Errors
    {
        public static class Common
        {
            public const string UnexpectedError = "Unexpected error occurred.";
            public const string ValidationFailed = "Validation failed";
            public const string NoDataFound = "No data found";
        }
        public static class SecureVault
        {
            public const string DatabaseConnectionStringNotFound = "Database connection string not found in Vault";
            public const string MinioCredentialsNotFound = "MinIO credentials not found in Vault";
            public const string NoSecretsFound = "No secrets found at path";
            public const string KeyNotFound = "Key not found in secret at path";
            public const string RetrievingMinio = "Error retrieving Minio credentials from Vault";
            public const string RetrievingSecrets = "Error retrieving secrets from Vault at path";
            public const string RetrievingDbConnectionString = "Error retrieving database connection string from Vault";
            public const string FailedToGetVaultConnection = "Failed to get connection string from Vault, using configuration fallback";
        }
        public static class Lighthouse
        {
            public const string LighthouseNotFound = "Lighthouse not found.";
            public const string FailedToDeleteLighthouse = "Failed to delete lighthouse.";
            public const string FailedToUpdateLighthouse = "Failed to update lighthouse.";
            public const string FailedToAddLighthouse = "Failed to add lighthouse to repository.";
            public const string NoLighthousesFound = "No lighthouses found.";
        }
        public static class Photo
        {
            public const string PhotoNotFound = "Photo not found.";
            public const string FailedToDeletePhoto = "Failed to delete photo from repository.";
            public const string FailedToAddPhoto = "Failed to add photo to repository.";
            public const string NoPhotosFoundForLighthouse = "No photos found for the specified lighthouse.";
            public const string PhotoNotFoundInStorage = "Photo not found in storage.";
            public const string NoCommentsFoundForPhoto = "No comments found for the specified photo.";
        }

        public static class Comment
        {
            public const string CommentNotFound = "Comment not found.";
            public const string FailedToDeleteComment = "Failed to delete comment.";
            public const string FailedToAddComment = "Failed to add comment to repository.";
        }

        public static class User
        {
            public const string UserNotFound = "User not found.";
        }
        public static class Country
        {
            public const string CountryNotFound = "Country not found.";
        }
    }
}