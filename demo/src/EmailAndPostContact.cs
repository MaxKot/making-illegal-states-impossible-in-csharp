using System;

namespace Demo
{
    /// <summary>
    /// Представляет контаные данные, содержащие и адрес электронной почты, и почтовый адрес.
    /// </summary>
    public sealed class EmailAndPostContact : Contact
    {
        /// <summary>Адрес электронной почты.</summary>
        private readonly EmailContactInfo email_;

        /// <summary>Почтовый адрес.</summary>
        private readonly PostalContactInfo post_;

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailAndPostContact"/>.
        /// </summary>
        /// <param name="name">Имя человека.</param>
        /// <param name="email">Адрес электронной почты.</param>
        /// <param name="post">Почтовый адрес.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="name"/> иммет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="email"/> имеет значение <see langword="null"/>.
        /// -или-
        /// Параметр <paramref name="post"/> имеет значение <see langword="null"/>.
        /// </exception>
        public EmailAndPostContact (PersonalName name, EmailContactInfo email, PostalContactInfo post)
            : base (name)
        {
            if (email == null)
            {
                throw new ArgumentNullException (nameof (email));
            }
            if (post == null)
            {
                throw new ArgumentNullException (nameof (post));
            }

            email_ = email;
            post_ = post;
        }

        /// <inheritdoc />
        public override Contact UpdatePostalAddress (PostalContactInfo newPostalAddress)
            => new EmailAndPostContact (Name, email_, newPostalAddress);

        /// <inheritdoc />
        public override void AcceptVisitor (IContactVisitor visitor)
        {
            if (visitor == null)
            {
                throw new ArgumentNullException (nameof (visitor));
            }

            visitor.Visit (Name, email_, post_);
        }

        /// <inheritdoc />
        public override string ToString ()
            => $"{base.ToString ()}: Email: {email_}; Post: {post_}";
    }
}
