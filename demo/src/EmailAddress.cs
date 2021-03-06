﻿using System;
using System.Text.RegularExpressions;

namespace Demo
{
    /// <summary>Представляет адрес электронной почты.</summary>
    public sealed class EmailAddress
    {
        /// <summary>
        /// Получает значение адреса.
        /// </summary>
        /// <value>Адрес электронной почты.</value>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="EmailAddress"/>.
        /// </summary>
        /// <param name="value">Адрес электронной почты.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="value"/> имеет значение <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Параметр <paramref name="value"/> не является корректным адресом электронной почты.
        /// </exception>
        public EmailAddress(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }
            if (!Regex.IsMatch(value, @"^\S+@\S+\.\S+$"))
            {
                throw new ArgumentException("Email address must contain an @ sign");
            }

            Value = value;
        }

        /// <inheritdoc />
        public override string ToString()
            => Value;

        /// <inheritdoc />
        public override bool Equals(object obj)
            => obj is EmailAddress otherEmailAddress &&
               Equals(Value, otherEmailAddress.Value);

        /// <inheritdoc />
        public override int GetHashCode()
            => Value?.GetHashCode() ?? 0;

        /// <summary>
        /// Осуществляет неявное преобразование значения типа <see cref="EmailAddress"/> к типу
        /// <see cref="String"/>.
        /// </summary>
        /// <param name="address">Адрес электронной почты.</param>
        /// <returns>Результат преобразования.</returns>
        public static implicit operator string(EmailAddress address)
            => address.Value;
    }
}
