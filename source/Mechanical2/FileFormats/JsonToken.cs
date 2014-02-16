using System;

namespace Mechanical.FileFormats
{
    /// <summary>
    /// Specifies core elements of the JSON format.
    /// </summary>
    public enum JsonToken : byte
    {
        /// <summary>
        /// The opening curly bracket of a JSON object.
        /// </summary>
        ObjectStart,

        /// <summary>
        /// The closing curly bracket of a JSON object.
        /// </summary>
        ObjectEnd,

        /// <summary>
        /// The opening bracket of a JSON array.
        /// </summary>
        ArrayStart,

        /// <summary>
        /// The closing bracket of a JSON array.
        /// </summary>
        ArrayEnd,

        /// <summary>
        /// The name of an item of a JSON object.
        /// </summary>
        Name,

        /// <summary>
        /// A non-null string value.
        /// </summary>
        StringValue,

        /// <summary>
        /// An integer, of floating-point number.
        /// </summary>
        NumberValue,

        /// <summary>
        /// A <see cref="Boolean"/> value.
        /// </summary>
        BooleanValue,

        /// <summary>
        /// A <c>null</c> value.
        /// </summary>
        NullValue,
    }
}
