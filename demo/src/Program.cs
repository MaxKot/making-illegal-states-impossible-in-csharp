
namespace Demo
{
    /// <summary>Представляет приложение.</summary>
    public static class Program
    {
        /// <summary>Задаёт точку входа в приложение.</summary>
        public static void Main()
        {
            var name = new PersonalName("A", null, "Smith");
            var contact = Contact.FromEmail(name, "abc@example.com");

            var state = new StateCode("CA");
            var zip = new ZipCode("97210");
            var newPostalAddress = new PostalAddress("123 Main", "", "Beverly Hills", state, zip);
            var newPostalContactInfo = new PostalContactInfo(newPostalAddress, false);
            var newContact = contact.UpdatePostalAddress(newPostalContactInfo);

            var ui = new ContactUi();
            ui.Display(newContact);
        }
    }
}
