﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;

namespace TheDotFactory
{
    // an output configuration preset
    public class OutputConfiguration
    {
        // padding removal type
        public enum PaddingRemoval
        {
            None,               // no padding removal
            Tighest,            // remove padding as much as possible, per bitmap
            Fixed               // remove padding as much as the bitmap with least padding
        }

        // Line wrap
        public enum LineWrap
        {
            AtColumn,           // After each column
            AtBitmap            // After each bitmap
        }

        // space generated
        public enum SpaceGeneration
        {
            AsBitmap,           // generate the space character normally
            Generated           // will be generated by driver, skip
        }

        // Comment style
        public enum CommentStyle
        {
            C,               // c style comments - /* */
            Cpp              // C++ style - //
        }

        // Indent style
        public enum IndentStyle
        {
            Tabs,
            Spaces
        }

        // Bit Layout
        public enum BitLayout
        {
            RowMajor,     // '|' = 0x80,0x80,0x80  '_' = 0x00,0x00,0xFF
            ColumnMajor,  // '|' = 0xFF,0x00,0x00  '_' = 0x80,0x80,0x80
        }

        // Byte Order
        public enum ByteOrder
        {
            LsbFirst,     // Least significat bit first
            MsbFirst      // Most significat bit first
        }

        // Byte format
        public enum ByteFormat
        {
            Binary,     // Binary
            Hex         // Hex
        }

        // rotation
        public enum Rotation
        {
            RotateZero,
            RotateNinety,
            RotateOneEighty,
            RotateTwoSeventy
        }

        // rotation display string
        public static readonly string[] RotationDisplayString = new string[]
        {
            "0°",
            "90°",
            "180°",
            "270°"
        };

        // rotation
        public enum DescriptorFormat
        {
            DontDisplay,
            DisplayInBits,
            DisplayInBytes
        }

        // rotation display string
        public static readonly string[] DescriptorFormatDisplayString = new string[]
        {
            "Don't display",
            "In bits",
            "In bytes"
        };

        // clone self
        public OutputConfiguration clone() { return (OutputConfiguration)this.MemberwiseClone(); }

        // leading strings
        public const string ByteLeadingStringBinary = "0b";
        public const string ByteLeadingStringHex = "0x";

        // comments
        public bool commentVariableName = true;
        public bool commentCharVisualizer = true;
        public bool commentCharDescriptor = true;
        public CommentStyle commentStyle = CommentStyle.Cpp;
        public String bmpVisualizerChar = "#";

        // whitespaces
        public IndentStyle indentStyle = IndentStyle.Tabs;
        public int numIndentSpaces = 2;

        // rotation
        public Rotation rotation = Rotation.RotateZero;

        // flip
        public bool flipHorizontal = false;
        public bool flipVertical = false;

        // padding removal
        public PaddingRemoval paddingRemovalHorizontal = PaddingRemoval.Fixed;
        public PaddingRemoval paddingRemovalVertical = PaddingRemoval.Tighest;

        // line wrap
        public LineWrap lineWrap = LineWrap.AtColumn;

        // byte
        public BitLayout bitLayout = BitLayout.RowMajor;
        public ByteOrder byteOrder = ByteOrder.MsbFirst;
        public ByteFormat byteFormat = ByteFormat.Hex;
        public string byteLeadingString = ByteLeadingStringHex;

        // descriptors
        public bool generateLookupArray = true;
        public DescriptorFormat descCharWidth = DescriptorFormat.DisplayInBits;
        public DescriptorFormat descCharHeight = DescriptorFormat.DontDisplay;
        public DescriptorFormat descFontHeight = DescriptorFormat.DisplayInBytes;
        public bool generateLookupBlocks = false;
        public int lookupBlocksNewAfterCharCount = 80;
        public DescriptorFormat descImgWidth = DescriptorFormat.DisplayInBytes;
        public DescriptorFormat descImgHeight = DescriptorFormat.DisplayInBits;

        // space generation
        public bool generateSpaceCharacterBitmap = false;
        public int spaceGenerationPixels = 2;

        // variable formats
        public string varNfBitmaps = "const uint_8 {0}Bitmaps";
        public string varNfCharInfo = "const FONT_CHAR_INFO {0}Descriptors";
        public string varNfFontInfo = "const FONT_INFO {0}FontInfo";
        public string varNfWidth = "const uint_8 {0}Width";
        public string varNfHeight = "const uint_8 {0}Height";

        // display name
        public string displayName = "";
    }

    // the output configuration manager
    public class OutputConfigurationManager
    {
        // add a configuration
        public int configurationAdd(ref OutputConfiguration configToAdd)
        {
            // add to list
            m_outputConfigurationList.Add(configToAdd);

            // return the index of the new item
            return m_outputConfigurationList.Count - 1;
        }

        // delete a configuration
        public void configurationDelete(int configIdxToRemove)
        {
            // check if in bounds
            if (configIdxToRemove >= 0 && configIdxToRemove < configurationCountGet())
            {
                // delete it
                m_outputConfigurationList.RemoveAt(configIdxToRemove);
            }
        }
        
        // get number of configurations
        public int configurationCountGet()
        {
            // get number of items
            return m_outputConfigurationList.Count;
        }

        // get configuration at index
        public OutputConfiguration configurationGetAtIndex(int index)
        {
            // return the configuration
            return m_outputConfigurationList[index];
        }

        // save to file
        public void saveToFile(string fileName)
        {
            // create serailizer and text writer
            XmlSerializer serializer = new XmlSerializer(m_outputConfigurationList.GetType());
            TextWriter textWriter = new StreamWriter(fileName);
            
            // serialize to xml
            serializer.Serialize(textWriter, m_outputConfigurationList);
            
            // close and flush the stream
            textWriter.Close();
        }

        // load from file
        public void loadFromFile(string fileName)
        {
            // create serailizer and text writer
            XmlSerializer serializer = new XmlSerializer(m_outputConfigurationList.GetType());

            // catch exceptions (especially file not found)
            try
            {
                // read text
                TextReader textReader = new StreamReader(fileName);

                // serialize to xml
                m_outputConfigurationList = (List<OutputConfiguration>)serializer.Deserialize(textReader);

                // close and flush the stream
                textReader.Close();
            }
            catch (IOException)
            {
            }
        }

        // populate the cbx
        public void comboboxPopulate(ComboBox combobox)
        {
            // clear all items
            combobox.Items.Clear();

            // iterate through items
            foreach (OutputConfiguration oc in m_outputConfigurationList)
            {
                // get the name
                combobox.Items.Add(oc.displayName);
            }
        }

        // a working copy configuration, used for when there are no presets and 
        // during editing
        public OutputConfiguration workingOutputConfiguration = new OutputConfiguration();

        // the output configuration
        private List<OutputConfiguration> m_outputConfigurationList = new List<OutputConfiguration>();
    }
}
