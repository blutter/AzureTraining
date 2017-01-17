using Microsoft.Azure.Mobile.Server;

namespace AzureGuestbook.Server.DataObjects
{
    public class Comment : EntityData
    {
        public string Description { get; set; }

        public string Name { get; set; }
    }
}