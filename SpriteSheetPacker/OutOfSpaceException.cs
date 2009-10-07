using System;
using System.Runtime.Serialization;

namespace SpriteSheetPacker
{
	/// <summary>Insufficient space left in packing area to contain a given object</summary>
	/// <remarks>
	///   An exception being sent to you from deep space. Erm, no, wait, it's an exception
	///   that occurs when a packing algorithm runs out of space and is unable to fit
	///   the object you tried to pack into the remaining packing area.
	/// </remarks>
	[Serializable]
	public class OutOfSpaceException : Exception 
	{
		/// <summary>Initializes the exception</summary>
		public OutOfSpaceException()
		{ 
		}

		/// <summary>Initializes the exception with an error message</summary>
		/// <param name="message">Error message describing the cause of the exception</param>
		public OutOfSpaceException(string message) 
			: base(message) 
		{ 
		}

		/// <summary>Initializes the exception as a followup exception</summary>
		/// <param name="message">Error message describing the cause of the exception</param>
		/// <param name="inner">Preceding exception that has caused this exception</param>
		public OutOfSpaceException(string message, Exception inner) 
			: base(message, inner) 
		{ 
		}

#if !XBOX
		/// <summary>Initializes the exception from its serialized state</summary>
		/// <param name="info">Contains the serialized fields of the exception</param>
		/// <param name="context">Additional environmental informations</param>
		protected OutOfSpaceException(SerializationInfo info, StreamingContext context)
			: base(info, context) 
		{ 
		}

#endif

	}
}