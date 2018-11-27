using System;

namespace Demo
{
    /// <summary>Представляет контаные данные, содержащие только адрес электронной почты.</summary>
    public sealed class EmailOnlyContact : Contact
    {
        /// <summary>Адрес электронной почты.</summary>
        private readonly EmailContactInfo email_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailOnlyContact"/>.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="email">Адрес электронной почты.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="name"/> иммет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="email"/> имеет значение <see langword="null"/>.
        /// </exception>
        public EmailOnlyContact(PersonalName name, EmailContactInfo email)
            : base(name)
        {
            if (email == null)
            {
                throw new ArgumentNullException(nameof(email));
            }

            email_ = email;
        }

        /// <inheritdoc />
        public override Contact UpdatePostalAddress(PostalContactInfo newPostalAddress)
            => new EmailAndPostContact(Name, email_, newPostalAddress);

        /// <inheritdoc />
        public override void AcceptVisitor(IContactVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException(nameof(visitor));
            }

            visitor.Visit(Name, email_);
        }

        /// <inheritdoc />
        public override string ToString()
            => $"{base.ToString()}: Email: {email_}";
    }
}
