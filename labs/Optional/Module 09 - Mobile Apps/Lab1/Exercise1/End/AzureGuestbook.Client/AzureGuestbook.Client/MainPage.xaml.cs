using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.System;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace AzureGuestbook.Client
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
        }


        private async void SubmitButton_Click(object sender, RoutedEventArgs e)
        {

            // Add new comment
            var commentText = CommentTextBox.Text;

            IReadOnlyList<User> users = await User.FindAllAsync();
            string displayName = "";
            foreach (User u in users)
            {
                var pFirstName = await u.GetPropertyAsync(KnownUserProperties.FirstName);
                var pLastName = await u.GetPropertyAsync(KnownUserProperties.LastName);
                displayName = string.Format("{0} {1}", pFirstName, pLastName);
            }
            var commentItem = new Comment { Name = displayName, Description = CommentTextBox.Text };
            await InsertCommentItem(commentItem);

        }

    }
}
