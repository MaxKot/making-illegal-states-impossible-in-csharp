using System;

namespace Demo
{
    /// <summary>Ассоциациирует человека с его контактными данными.</summary>
    public abstract class Contact
    {
        /// <summary>
        /// Возвращает имя человека.
        /// </summary>
        /// <value>Имя человека.</value>
        public PersonalName Name { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="Contact"/>.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <exception cref="ArgumentNullException">
        /// <paramref name="name"/> иммет значение <see langword="null"/>.
        /// </exception>
        protected Contact(PersonalName name)
        {
            if (name == null)
            {
                throw new ArgumentNullException(nameof(name));
            }

            Name = name;
        }

        /// <summary>
        /// Производит двойную диспетчеризацию в зависимости от реализации
        /// <see cref="Contact"/>.
        /// </summary>
        /// <param name="visitor">
        /// Объект, обрабатывающий вызовы конкретных реализации <see cref="Contact"/>.
        /// </param>
        public abstract void AcceptVisitor(IContactVisitor visitor);

        /// <summary>
        /// Создаёт ассоциацию укказанного человека с адресом электронной почты.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="emailStr">Адрес электронной почты указанного человека.</param>
        /// <returns>Созданный контакт.</returns>
        public static Contact FromEmail(PersonalName name, string emailStr)
        {
            var email = new EmailAddress(emailStr);
            var emailContactInfo = new EmailContactInfo(email, false);
            return new EmailOnlyContact(name, emailContactInfo);
        }

        /// <summary>
        /// Обновляет почтовый адрес в контактных данных.
        /// </summary>
        /// <param name="newPostalAddress">Новый почтовый адрес.</param>
        /// <returns>Контакт с обновлённым почтовым адресом.</returns>
        public abstract Contact UpdatePostalAddress(PostalContactInfo newPostalAddress);

        /// <inheritdoc />
        public override string ToString()
            => $"{Name}";
    }
}
