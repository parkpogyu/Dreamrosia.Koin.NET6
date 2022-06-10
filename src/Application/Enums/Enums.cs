using System.ComponentModel;

namespace Dreamrosia.Koin.Application.Enums
{
    public enum AuditType : byte
    {
        None = 0,
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum UploadType : byte
    {
        [Description(@"Images\Products")]
        Product,

        [Description(@"Images\ProfilePictures")]
        ProfilePicture,

        [Description(@"Documents")]
        Document
    }

    public enum ServerModes
    {
        Server,
        Agent,
        Test
    }
}
