using System;

namespace Demo
{
    /// <summary>Представляет почтовый индекс США.</summary>
    public sealed class ZipCode
    {
        /// <summary>
        /// Получается значение индекса.
        /// </summary>
        /// <value>Почтовый индекс.</value>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="ZipCode"/>.
        /// </summary>
        /// <param name="value">Почтовый индекс.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="value"/> имеет значение <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Параметр <paramref name="value"/> не является корректным почтовым индексом.
        /// </exception>
        public ZipCode (string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException (nameof (value));
            }
            if (!System.Text.RegularExpressions.Regex.IsMatch (value, @"^\d{5}$"))
            {
                throw new ArgumentException ("Zip code must be 5 digits");
            }

            Value = value;
        }

        /// <inheritdoc />
        public override string ToString ()
            => Value;

        /// <inheritdoc />
        public override bool Equals (object other)
            => other is ZipCode otherZipCode &&
               Value.Equals (otherZipCode.Value);

        /// <inheritdoc />
        public override int GetHashCode ()
            => Value.GetHashCode ();

        /// <summary>
        /// Осуществляет неявное преобразование значения типа <see cref="ZipCode"/> к типу
        /// <see cref="String"/>.
        /// </summary>
        /// <param name="address">Почтовый индекс.</param>
        /// <returns>Результат преобразования.</returns>
        public static implicit operator string (ZipCode zipCode)
            => zipCode?.Value;
    }
}
