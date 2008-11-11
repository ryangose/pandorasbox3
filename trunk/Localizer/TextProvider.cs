using System;
// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
using System.Collections.Generic;
// Issue 10 - End
using System.Xml.Serialization;
using System.IO;
using System.Xml;
using System.Reflection;

namespace TheBox.Lang
{
	[ Serializable ]
	/// <summary>
	/// Provides localized text elements for the box
	/// </summary>
	public class TextProvider
	{
		/// <summary>
		/// Gets the text associated with the specified resource
		/// </summary>
		public string this[string description]
		{
			get
			{
				string[] locate = description.Split( new char[] { '.' } );

				if ( locate.Length != 2 )
				{
					return null;
				}
				// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
				Dictionary<string, string> loc;
				m_Sections.TryGetValue(locate[0], out loc);
				// Issue 10 - End

				if ( loc == null )
					return null;
				// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
				string s;
				loc.TryGetValue(locate[1], out s);
				return s;
				// Issue 10 - End
			}
			set
			{
				string[] locate = description.Split( new char[] { '.' } );

				if ( locate.Length != 2 )
				{
					throw new Exception( "Bad descriptor when adding a new entry to text provider" );
				}

				Add( value, locate[0], locate[1] );
			}
		}
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		private Dictionary<string, Dictionary<string, string>> m_Sections;
		// Issue 10 - End
		private string m_Language;

		/// <summary>
		/// Gets or sets a string identifying the language represented by the text provider
		/// </summary>
		public string Language
		{
			get { return m_Language; }
			set { m_Language = value; }
		}

		/// <summary>
		/// Gets or sets the data collection for this text provider
		/// </summary>
		// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
		public Dictionary<string, Dictionary<string, string>> Data
		// Issue 10 - End
		{
			get { return m_Sections; }
			set { m_Sections = value; }
		}

		/// <summary>
		/// Creates a new TextProvider object
		/// </summary>
		public TextProvider()
		{
			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			m_Sections = new Dictionary<string, Dictionary<string,string>>();
			// Issue 10 - End
		}

		private void Add( string text, string category, string definition )
		{
			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			Dictionary<string, string> loc = null;
			

			if ( m_Sections.ContainsKey( category ) )
			{
				loc = m_Sections[category];
			}
			else
			{
				loc = new Dictionary<string, string>();

				m_Sections.Add( category, loc );
			}
			
			if(loc.ContainsKey(definition))
			{
				loc[definition] = text;
			}
			// Issue 10 - End
		}

		/// <summary>
		/// Deletes a section contained in the TextProvider
		/// </summary>
		/// <param name="name">The name of the section that will be deleted</param>
		public void DeleteSection( string name )
		{
			m_Sections.Remove( name );
		}

		/// <summary>
		/// Removes an item from the TextProvider
		/// </summary>
		/// <param name="section">The section the item belongs to</param>
		/// <param name="item">The item name</param>
		public void RemoveItem( string section, string item )
		{
			// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
			Dictionary<string, string> hash; 
			m_Sections.TryGetValue(section, out hash);
			// Issue 10 - End

			if ( hash != null )
			{
				hash.Remove( item );
			}
		}

		/// <summary>
		/// Removes an item from the TextProvider
		/// </summary>
		/// <param name="definition">The full item definition</param>
		public void RemoveItem( string definition )
		{
			string[] loc = definition.Split( new char[] { '.' } );

			if ( loc.Length != 2 )
				return;

			RemoveItem( loc[0], loc[1] );
		}

		/// <summary>
		/// Saves the contents of the TextProvider to file
		/// </summary>
		/// <param name="filename"></param>
		public void Serialize( string filename )
		{
			XmlDocument dom = new XmlDocument();

			XmlNode decl = dom.CreateXmlDeclaration( "1.0", null, null );

			dom.AppendChild( decl );

			XmlNode lang = dom.CreateElement( "Data" );

			XmlAttribute langtype = dom.CreateAttribute( "language" );
			langtype.Value = m_Language;
			lang.Attributes.Append( langtype );

			foreach ( string toplevel in m_Sections.Keys )
			{
				XmlNode topnode = dom.CreateElement( "section" );

				XmlAttribute topname = dom.CreateAttribute( "name" );
				topname.Value = toplevel;

				topnode.Attributes.Append( topname );

				// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
				Dictionary<string, string> hash;
				m_Sections.TryGetValue(toplevel, out hash);
				// Issue 10 - End

				foreach( string lowlevel in hash.Keys )
				{
					XmlNode entrynode = dom.CreateElement( "entry" );

					XmlAttribute name = dom.CreateAttribute( "name" );
					name.Value = lowlevel;
					entrynode.Attributes.Append( name );

					XmlAttribute val = dom.CreateAttribute( "text" );
					// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
					string value;
					hash.TryGetValue(lowlevel, out value);
					val.Value = value;
					// Issue 10 - End
					entrynode.Attributes.Append( val );

					topnode.AppendChild( entrynode );
				}
				
				lang.AppendChild( topnode );
			}

			dom.AppendChild( lang );

			dom.Save( filename );
		}

		/// <summary>
		/// Reads a TextProvider item from an Xml document
		/// </summary>
		/// <param name="dom">The XmlDocument containing the object</param>
		/// <returns>A TextProvider object</returns>
		public static TextProvider Deserialize( XmlDocument dom )
		{
			XmlNode data = dom.ChildNodes[ 1 ];

			TextProvider text = new TextProvider();

			text.m_Language = data.Attributes[ "language" ].Value;

			foreach ( XmlNode section in data.ChildNodes )
			{
				string topkey = section.Attributes[ "name" ].Value;
				// Issue 10 - Update the code to Net Framework 3.5 - http://code.google.com/p/pandorasbox3/issues/detail?id=10 - Smjert
				Dictionary<string, string> hash = new Dictionary<string, string>();
				// Issue 10 - End

				foreach ( XmlNode entry in section.ChildNodes )
				{
					string lowkey = entry.Attributes[ "name" ].Value;
					string t = entry.Attributes[ "text" ].Value;

					hash.Add( lowkey, t );
				}

				text.m_Sections.Add( topkey, hash );
			}

			return text;
		}
	}
}