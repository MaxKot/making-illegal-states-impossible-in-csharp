using System;

namespace Demo
{
    /// <summary>Представляет код штата США.</summary>
    public sealed class StateCode
    {
        /// <summary>
        /// Получается значение кода штата.
        /// </summary>
        /// <value>Код штата.</value>
        public string Value { get; }

        /// <summary>
        /// Инициализирует новый экземпляр класса <see cref="StateCode"/>.
        /// </summary>
        /// <param name="value">Код штата.</param>
        /// <exception cref="ArgumentNullException">
        /// Параметр <paramref name="value"/> имеет значение <see langword="null"/>.
        /// </exception>
        /// <exception cref="ArgumentException">
        /// Параметр <paramref name="value"/> не является корректным кодом штата.
        /// </exception>
        public StateCode(string value)
        {
            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            var valueNormalized = value.ToUpper();
            if (Array.IndexOf(StateCodes, valueNormalized) < 0)
            {
                throw new ArgumentException("State is not in list");
            }

            Value = valueNormalized;
        }

        /// <summary>Корректные коды штата.</summary>
        private static readonly string[] StateCodes = { "AZ", "CA", "NY" }; // и т.д.

        /// <inheritdoc />
        public override string ToString()
            => Value;

        /// <inheritdoc />
        public override bool Equals(object other)
            => other is StateCode otherStateCode &&
               Value.Equals(otherStateCode.Value);

        /// <inheritdoc />
        public override int GetHashCode()
            => Value.GetHashCode();

        /// <summary>
        /// Осуществляет неявное преобразование значения типа <see cref="StateCode"/> к типу
        /// <see cref="String"/>.
        /// </summary>
        /// <param name="address">Код штата.</param>
        /// <returns>Результат преобразования.</returns>
        public static implicit operator string(StateCode stateCode)
            => stateCode?.Value;
    }
}
