using System;

namespace Demo
{
    /// <summary>Представляет контаные данные, содержащие только адрес электронной почты.</summary>
    public sealed class EmailOnlyContactInfo : ContactInfo
    {
        /// <summary>Адрес электронной почты.</summary>
        private readonly EmailContactInfo email_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailOnlyContactInfo"/>.
        /// </summary>
        /// <param name="email">Адрес электронной почты.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="email"/> имеет значение <see langword="null"/>.
        /// </exception>
        public EmailOnlyContactInfo (EmailContactInfo email)
        {
            if (email == null)
            {
                throw new ArgumentNullException (nameof (email));
            }

            email_ = email;
        }

        /// <inheritdoc />
        public override void AcceptVisitor (IContactInfoVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException (nameof (visitor));
            }

            visitor.Visit (email_);
        }

        /// <inheritdoc />
        public override string ToString ()
            => $"Email: {email_}";
    }
}
