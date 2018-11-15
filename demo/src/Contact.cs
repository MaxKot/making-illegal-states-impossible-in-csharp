using System;

namespace Demo
{
    /// <summary>Ассоциациирует человека с его контактными данными.</summary>
    public sealed class Contact
    {
        /// <summary>
        /// Возвращает имя человека.
        /// </summary>
        /// <value>Имя человека.</value>
        public PersonalName Name { get; }

        /// <summary>
        /// Возвращает контактные данные человека.
        /// </summary>
        /// <value>Контактные данные человека.</value>
        public ContactInfo ContactInfo { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/>.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="contactInfo">Контактные данные человека.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> иммет значение <see langword="null"/>.
        /// или
        /// <paramref name="contactInfo"/> иммет значение <see langword="null"/>.
        /// </exception>
        public Contact (PersonalName name, ContactInfo contactInfo)
        {
            if (name == null)
            {
                throw new ArgumentNullException (nameof (name));
            }
            if (contactInfo == null)
            {
                throw new ArgumentNullException (nameof (contactInfo));
            }

            Name = name;
            ContactInfo = contactInfo;
        }

        /// <summary>
        /// Создаёт ассоциацию укказанного человека с адресом электронной почты.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="emailStr">Адрес электронной почты указанного человека.</param>
        /// <returns>Созданный контакт.</returns>
        public static Contact FromEmail (PersonalName name, string emailStr)
        {
            var email = new EmailAddress (emailStr);
            var emailContactInfo = new EmailContactInfo (email, false);
            var contactInfo = new EmailOnlyContactInfo (emailContactInfo);
            return new Contact (name, contactInfo);
        }

        /// <summary>Добавляет либо заменяет почтовый адрес в контактных данных.</summary>
        private sealed class PostalAddressUpdater : IContactInfoVisitor
        {
            /// <summary>
            /// Возвращает обновлённые контактные данные.
            /// </summary>
            /// <value>
            /// Обновлённые контактные данные или <see langword="null"/>, если обновление не
            /// применялось ни к каким контактным данным.
            /// </value>
            public ContactInfo UpdatedContactInfo { get; private set; }

            /// <summary>Новый почтовый адрес.</summary>
            private readonly PostalContactInfo newPostalAddress_;

            /// <summary>
            /// Инициализирует новый экземпляр класса <see cref="PostalAddressUpdater"/>.
            /// </summary>
            /// <param name="newPostalAddress">Новый почтовый адрес.</param>
            public PostalAddressUpdater (PostalContactInfo newPostalAddress)
            {
                System.Diagnostics.Debug.Assert (newPostalAddress != null);
                UpdatedContactInfo = null;
                newPostalAddress_ = newPostalAddress;
            }

            /// <inheritdoc />
            public void Visit (EmailContactInfo email)
            {
                System.Diagnostics.Debug.Assert (email != null);
                UpdatedContactInfo = new EmailAndPostContactInfo (email, newPostalAddress_);
            }

            /// <inheritdoc />
            public void Visit (PostalContactInfo _)
            {
                UpdatedContactInfo = new PostOnlyContactInfo (newPostalAddress_);
            }

            /// <inheritdoc />
            public void Visit (EmailContactInfo email, PostalContactInfo _)
            {
                System.Diagnostics.Debug.Assert (email != null);
                UpdatedContactInfo = new EmailAndPostContactInfo (email, newPostalAddress_);
            }
        }

        /// <summary>
        /// Обновляет почтовый адрес в контактных данных.
        /// </summary>
        /// <param name="newPostalAddress">Новый почтовый адрес.</param>
        /// <returns>Контакт с обновлённым почтовым адресом.</returns>
        public Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
        {
            var updater = new PostalAddressUpdater (newPostalAddress);
            ContactInfo.AcceptVisitor (updater);
            return new Contact (Name, updater.UpdatedContactInfo);
        }

        /// <inheritdoc />
        public override string ToString ()
            => $"{Name}: {ContactInfo}";
    }
}
