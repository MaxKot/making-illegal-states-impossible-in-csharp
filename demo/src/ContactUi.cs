using System;

namespace Demo
{
    /// <summary>
    /// Представляет пользовательский интерфейс отображения контактов.
    /// </summary>
    public sealed class ContactUi
    {
        /// <summary>
        /// Отображает контакт в соответствии с его типом.
        /// </summary>
        private sealed class Visitor : IContactVisitor
        {
            /// <inheritdoc />
            void IContactVisitor.Visit (PersonalName name, EmailContactInfo email)
            {
                Console.WriteLine (name);
                Console.WriteLine ("* Email: {0}", email);
            }

            /// <inheritdoc />
            void IContactVisitor.Visit (PersonalName name, PostalContactInfo post)
            {
                Console.WriteLine (name);
                Console.WriteLine ("* Postal address: {0}", post);
            }

            /// <inheritdoc />
            void IContactVisitor.Visit (PersonalName name, EmailContactInfo email, PostalContactInfo post)
            {
                Console.WriteLine (name);
                Console.WriteLine ("* Email: {0}", email);
                Console.WriteLine ("* Postal address: {0}", post);
            }
        }

        /// <summary>
        /// Отображает указанный контакт.
        /// </summary>
        /// <param name="contact">Контакт.</param>
        public void Display (Contact contact)
            => contact.AcceptVisitor (new Visitor ());
    }
}
