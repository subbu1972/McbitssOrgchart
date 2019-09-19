using System;
using System.Web;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Office.Drawing;
using DocumentFormat.OpenXml.Office2010.Drawing;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Presentation;
using DocumentFormat.OpenXml.Validation;
using A = DocumentFormat.OpenXml.Drawing;
using P = DocumentFormat.OpenXml.Presentation;

using REORGCHART.Models;
using System.Text.RegularExpressions;

namespace REORGCHART
{
    public static class StringExtensions
    {
        public static string SafeReplace(this string input, string find, string replace, bool matchWholeWord)
        {
            string textToFind = matchWholeWord ? string.Format(@"\b{0}\b", find) : find;
            return Regex.Replace(input, textToFind, replace);
        }
    }

    public class InsertImage
    {
        /// <summary>
        /// Insert a new Slide into PowerPoint
        /// </summary>
        /// <param name="presentationPart">Presentation Part</param>
        /// <param name="layoutName">Layout of the new Slide</param>
        /// <returns>Slide Instance</returns>
        public Slide InsertSlide(PresentationPart presentationPart, string layoutName)
        {
            UInt32 slideId = 256U;

            // Get the Slide Id collection of the presentation document
            var slideIdList = presentationPart.Presentation.SlideIdList;

            if (slideIdList == null)
            {
                throw new NullReferenceException("The number of slide is empty, please select a ppt with a slide at least again");
            }

            slideId += Convert.ToUInt32(slideIdList.Count());

            // Creates a Slide instance and adds its children.
            Slide slide = new Slide(new CommonSlideData(new P.ShapeTree()));

            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();
            slide.Save(slidePart);

            // Get SlideMasterPart and SlideLayoutPart from the existing Presentation Part
            SlideMasterPart slideMasterPart = presentationPart.SlideMasterParts.First();
            SlideLayoutPart slideLayoutPart = slideMasterPart.SlideLayoutParts.SingleOrDefault
                (sl => sl.SlideLayout.CommonSlideData.Name.Value.Equals(layoutName, StringComparison.OrdinalIgnoreCase));
            if (slideLayoutPart == null)
            {
                throw new Exception("The slide layout " + layoutName + " is not found");
            }

            slidePart.AddPart<SlideLayoutPart>(slideLayoutPart);

            slidePart.Slide.CommonSlideData = (CommonSlideData)slideMasterPart.SlideLayoutParts.SingleOrDefault(
                sl => sl.SlideLayout.CommonSlideData.Name.Value.Equals(layoutName)).SlideLayout.CommonSlideData.Clone();

            // Create SlideId instance and Set property
            SlideId newSlideId = presentationPart.Presentation.SlideIdList.AppendChild<SlideId>(new SlideId());
            newSlideId.Id = slideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            return GetSlideByRelationShipId(presentationPart, newSlideId.RelationshipId);
        }

        /// <summary>
        /// Get Slide By RelationShip ID
        /// </summary>
        /// <param name="presentationPart">Presentation Part</param>
        /// <param name="relationshipId">Relationship ID</param>
        /// <returns>Slide Object</returns>
        private static Slide GetSlideByRelationShipId(PresentationPart presentationPart, StringValue relationshipId)
        {
            // Get Slide object by Relationship ID
            SlidePart slidePart = presentationPart.GetPartById(relationshipId) as SlidePart;
            if (slidePart != null)
            {
                return slidePart.Slide;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Insert Image into Slide
        /// </summary>
        /// <param name="filePath">PowerPoint Path</param>
        /// <param name="imagePath">Image Path</param>
        /// <param name="imageExt">Image Extension</param>
        public void InsertImageInLastSlide(Slide slide, string imagePath, string imageExt)
        {
            // Creates a Picture instance and adds its children.
            P.Picture picture = new P.Picture();
            string embedId = string.Empty;
            embedId = "rId" + (slide.Elements<P.Picture>().Count() + 915).ToString();
            P.NonVisualPictureProperties nonVisualPictureProperties = new P.NonVisualPictureProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = "Picture 5" },
                new P.NonVisualPictureDrawingProperties(new A.PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());

            P.BlipFill blipFill = new P.BlipFill();
            Blip blip = new Blip() { Embed = embedId };

            // Creates a BlipExtensionList instance and adds its children
            BlipExtensionList blipExtensionList = new BlipExtensionList();
            BlipExtension blipExtension = new BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };

            UseLocalDpi useLocalDpi = new UseLocalDpi() { Val = false };
            useLocalDpi.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");

            blipExtension.Append(useLocalDpi);
            blipExtensionList.Append(blipExtension);
            blip.Append(blipExtensionList);

            Stretch stretch = new Stretch();
            FillRectangle fillRectangle = new FillRectangle();
            stretch.Append(fillRectangle);

            blipFill.Append(blip);
            blipFill.Append(stretch);

            // Creates a ShapeProperties instance and adds its children.
            P.ShapeProperties shapeProperties = new P.ShapeProperties();

            A.Transform2D transform2D = new A.Transform2D();
            A.Offset offset = new A.Offset() { X = 457200L, Y = 500000L };
            A.Extents extents = new A.Extents() { Cx = 13229600L, Cy = 9529200L };

            transform2D.Append(offset);
            transform2D.Append(extents);

            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList = new A.AdjustValueList();

            presetGeometry.Append(adjustValueList);

            shapeProperties.Append(transform2D);
            shapeProperties.Append(presetGeometry);

            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);

            slide.CommonSlideData.ShapeTree.AppendChild(picture);

            // Generates content of imagePart.
            ImagePart imagePart = slide.SlidePart.AddNewPart<ImagePart>(imageExt, embedId);
            FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
        }

        // Get the presentation object and pass it to the next CountSlides method.
        public int CountSlides(string presentationFile)
        {
            // Open the presentation as read-only.
            using (PresentationDocument presentationDocument = PresentationDocument.Open(presentationFile, false))
            {
                // Pass the presentation to the next CountSlide method
                // and return the slide count.
                return CountSlides(presentationDocument);
            }
        }

        // Count the slides in the presentation.
        public int CountSlides(PresentationDocument presentationDocument)
        {
            // Check for a null document object.
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            int slidesCount = 0;

            // Get the presentation part of document.
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the slide count from the SlideParts.
            if (presentationPart != null)
            {
                slidesCount = presentationPart.SlideParts.Count();
            }

            // Return the slide count to the previous method.
            return slidesCount;
        }

        // Get the presentation object and pass it to the next DeleteSlide method.
        public void DeleteSlide(string presentationFile, int slideIndex)
        {
            // Open the source document as read/write.

            using (PresentationDocument presentationDocument = PresentationDocument.Open(presentationFile, true))
            {
                // Pass the source document and the index of the slide to be deleted to the next DeleteSlide method.
                DeleteSlide(presentationDocument, slideIndex);
            }
        }

        // Delete the specified slide from the presentation.
        public void DeleteSlide(PresentationDocument presentationDocument, int slideIndex)
        {
            if (presentationDocument == null)
            {
                throw new ArgumentNullException("presentationDocument");
            }

            // Use the CountSlides sample to get the number of slides in the presentation.
            int slidesCount = CountSlides(presentationDocument);

            if (slideIndex < 0 || slideIndex >= slidesCount)
            {
                throw new ArgumentOutOfRangeException("slideIndex");
            }

            // Get the presentation part from the presentation document. 
            PresentationPart presentationPart = presentationDocument.PresentationPart;

            // Get the presentation from the presentation part.
            Presentation presentation = presentationPart.Presentation;

            // Get the list of slide IDs in the presentation.
            SlideIdList slideIdList = presentation.SlideIdList;

            // Get the slide ID of the specified slide
            SlideId slideId = slideIdList.ChildElements[slideIndex] as SlideId;

            // Get the relationship ID of the slide.
            string slideRelId = slideId.RelationshipId;

            // Remove the slide from the slide list.
            slideIdList.RemoveChild(slideId);

            // Remove references to the slide from all custom shows.
            if (presentation.CustomShowList != null)
            {
                // Iterate through the list of custom shows.
                foreach (var customShow in presentation.CustomShowList.Elements<CustomShow>())
                {
                    if (customShow.SlideList != null)
                    {
                        // Declare a link list of slide list entries.
                        System.Collections.Generic.LinkedList<SlideListEntry> slideListEntries = new System.Collections.Generic.LinkedList<SlideListEntry>();
                        foreach (SlideListEntry slideListEntry in customShow.SlideList.Elements())
                        {
                            // Find the slide reference to remove from the custom show.
                            if (slideListEntry.Id != null && slideListEntry.Id == slideRelId)
                            {
                                slideListEntries.AddLast(slideListEntry);
                            }
                        }

                        // Remove all references to the slide from the custom show.
                        foreach (SlideListEntry slideListEntry in slideListEntries)
                        {
                            customShow.SlideList.RemoveChild(slideListEntry);
                        }
                    }
                }
            }

            // Save the modified presentation.
            presentation.Save();

            // Get the slide part for the specified slide.
            SlidePart slidePart = presentationPart.GetPartById(slideRelId) as SlidePart;

            // Remove the slide part.
            presentationPart.DeletePart(slidePart);
        }


        // Insert the specified slide into the presentation at the specified position.
        public void InsertNewSlide(PresentationPart presentationPart)
        {
            // Verify that the presentation is not empty.
            if (presentationPart == null)
            {
                throw new InvalidOperationException("The presentation document is empty.");
            }

            // Declare and instantiate a new slide.
            Slide slide = new Slide(new CommonSlideData(new P.ShapeTree()));

            // Specify the group shape properties of the new slide.
            slide.CommonSlideData.ShapeTree.AppendChild(new P.GroupShapeProperties());

            // Create the slide part for the new slide.
            SlidePart slidePart = presentationPart.AddNewPart<SlidePart>();

            // Save the new slide part.
            slide.Save(slidePart);

            // Modify the slide ID list in the presentation part.
            // The slide ID list should not be null.
            SlideIdList slideIdList = presentationPart.Presentation.SlideIdList;

            // Find the highest slide ID in the current list.
            uint maxSlideId = Convert.ToUInt32(slideIdList.ChildElements.Count());
            int position = slideIdList.ChildElements.Count();
            SlideId prevSlideId = null;

            foreach (SlideId slideId in slideIdList.ChildElements)
            {
                if (slideId.Id > maxSlideId)
                {
                    maxSlideId = slideId.Id;
                }

                position--;
                if (position == 0)
                {
                    prevSlideId = slideId;
                }

            }

            maxSlideId++;

            // Get the ID of the previous slide.
            SlidePart lastSlidePart;

            if (prevSlideId != null)
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(prevSlideId.RelationshipId);
            }
            else
            {
                lastSlidePart = (SlidePart)presentationPart.GetPartById(((SlideId)(slideIdList.ChildElements[0])).RelationshipId);
            }

            // Use the same slide layout as that of the previous slide.
            if (null != lastSlidePart.SlideLayoutPart)
            {
                slidePart.AddPart(lastSlidePart.SlideLayoutPart);
            }

            // Insert the new slide into the slide list after the previous slide.
            SlideId newSlideId = slideIdList.InsertAfter(new SlideId(), prevSlideId);
            newSlideId.Id = maxSlideId;
            newSlideId.RelationshipId = presentationPart.GetIdOfPart(slidePart);

            // Save the modified presentation.
            presentationPart.Presentation.Save();
        }

        uint uniqueId;
        public void MergeSlides(string presentationFolder, string sourcePresentation, string destPresentation, int id, int Index)
        {
            int Idx = 0;

            // Open the destination presentation.
            using (PresentationDocument myDestDeck = PresentationDocument.Open(presentationFolder + destPresentation, true))
            {
                PresentationPart destPresPart = myDestDeck.PresentationPart;

                // If the merged presentation does not have a SlideIdList 
                // element yet, add it.
                if (destPresPart.Presentation.SlideIdList == null)
                    destPresPart.Presentation.SlideIdList = new SlideIdList();

                // Open the source presentation. This will throw an exception if
                // the source presentation does not exist.
                using (PresentationDocument mySourceDeck = PresentationDocument.Open(presentationFolder + sourcePresentation, false))
                {
                    PresentationPart sourcePresPart = mySourceDeck.PresentationPart;

                    // Get unique ids for the slide master and slide lists for use later.
                    uniqueId = GetMaxSlideMasterId(destPresPart.Presentation.SlideMasterIdList);
                    uint maxSlideId = GetMaxSlideId(destPresPart.Presentation.SlideIdList);

                    // Copy each slide in the source presentation, in order, to the destination presentation.
                    foreach (SlideId slideId in sourcePresPart.Presentation.SlideIdList)
                    {
                        if (Idx++ == Index)
                        {
                            SlidePart sp;
                            SlidePart destSp;
                            SlideMasterPart destMasterPart;
                            string relId;
                            SlideMasterId newSlideMasterId;
                            SlideId newSlideId;

                            // Create a unique relationship id.
                            id++;
                            sp = (SlidePart)sourcePresPart.GetPartById(slideId.RelationshipId);
                            relId = sourcePresentation.Remove(sourcePresentation.IndexOf('.')) + id;

                            // Add the slide part to the destination presentation.
                            destSp = destPresPart.AddPart<SlidePart>(sp, relId);

                            // The slide master part was added. Make sure the
                            // relationship between the main presentation part and
                            // the slide master part is in place.
                            destMasterPart = destSp.SlideLayoutPart.SlideMasterPart;
                            destPresPart.AddPart(destMasterPart);

                            // Add the slide master id to the slide master id list.
                            uniqueId++;
                            newSlideMasterId = new SlideMasterId();
                            newSlideMasterId.RelationshipId = destPresPart.GetIdOfPart(destMasterPart);
                            newSlideMasterId.Id = uniqueId;

                            destPresPart.Presentation.SlideMasterIdList.Append(newSlideMasterId);

                            // Add the slide id to the slide id list.
                            maxSlideId++;
                            newSlideId = new SlideId();
                            newSlideId.RelationshipId = relId;
                            newSlideId.Id = maxSlideId;

                            destPresPart.Presentation.SlideIdList.Append(newSlideId);
                        }
                    }

                    // Make sure that all slide layout ids are unique.
                    FixSlideLayoutIds(destPresPart);
                }

                // Save the changes to the destination deck.
                destPresPart.Presentation.Save();
            }
        }

        private void FixSlideLayoutIds(PresentationPart presPart)
        {
            // Make sure that all slide layouts have unique ids.
            foreach (SlideMasterPart slideMasterPart in presPart.SlideMasterParts)
            {
                foreach (SlideLayoutId slideLayoutId in slideMasterPart.SlideMaster.SlideLayoutIdList)
                {
                    uniqueId++;
                    slideLayoutId.Id = (uint)uniqueId;
                }

                slideMasterPart.SlideMaster.Save();
            }
        }

        private uint GetMaxSlideId(SlideIdList slideIdList)
        {
            // Slide identifiers have a minimum value of greater than or
            // equal to 256 and a maximum value of less than 2147483648. 
            uint max = 256;

            if (slideIdList != null)
            {
                // Get the maximum id value from the current set of children.
                foreach (SlideId child in slideIdList.Elements<SlideId>())
                {
                    uint id = child.Id;
                    if (id > max) max = id;
                }
            }

            return max;
        }

        private uint GetMaxSlideMasterId(SlideMasterIdList slideMasterIdList)
        {
            // Slide master identifiers have a minimum value of greater than
            // or equal to 2147483648. 
            uint max = 2147483648;

            if (slideMasterIdList != null)
            {
                // Get the maximum id value from the current set of children.
                foreach (SlideMasterId child in slideMasterIdList.Elements<SlideMasterId>())
                {
                    uint id = child.Id;

                    if (id > max)
                        max = id;
                }
            }

            return max;
        }

        static void DisplayValidationErrors(IEnumerable<ValidationErrorInfo> errors)
        {
            int errorIndex = 1;

            foreach (ValidationErrorInfo errorInfo in errors)
            {
                Console.WriteLine(errorInfo.Description);
                Console.WriteLine(errorInfo.Path.XPath);

                if (++errorIndex <= errors.Count())
                    Console.WriteLine("================");
            }
        }

        private int FieldPosition(DataTable dtFieldActive, string FieldName)
        {
            int Index = 0;
            foreach (DataRow dr in dtFieldActive.Rows)
            {
                if (dr["FIELD_NAME"].ToString() == FieldName) return Index;
                Index++;
            }

            return -1;
        }

        // Replace image in PPT shape.
        public void ReplaceFirstImageMatchingAltText(PresentationDocument presentationDocument, string newImage, string alternateTextToFind,
                                                     string newImagePath, List<string> LstShowPicture, List<string> LstShowText,
                                                     int SlideIndex, DataTable dtFieldActive, List<PPTXTemplateFields> PPTXAllparams)
        {
            try
            {
                int Idx = SlideIndex * 1000, Idz = 0, TextIndex = 0;
                OpenXmlElementList slideIds = presentationDocument.PresentationPart.Presentation.SlideIdList.ChildElements;

                foreach (SlideId sID in slideIds) // loop thru the SlideIDList
                {
                    if (Idz++ == SlideIndex)
                    {
                        string relId = sID.RelationshipId; // get first slide relationship
                        SlidePart slide = (SlidePart)presentationDocument.PresentationPart.GetPartById(relId); // Get the slide part from the relationship ID. 

                        var pictures = slide.Slide.Descendants<P.ShapeTree>().First().Descendants<P.Picture>().ToList();
                        foreach (var picture in pictures)
                        {
                            // get photo desc to see if it matches search text 
                            var nonVisualPictureProperties = picture.Descendants<P.NonVisualPictureProperties>().FirstOrDefault();
                            if (nonVisualPictureProperties == null)
                                continue;
                            var nonVisualDrawingProperties99 = nonVisualPictureProperties.Descendants<P.NonVisualDrawingProperties>().FirstOrDefault();
                            if (nonVisualDrawingProperties99 == null)
                                continue;
                            var desc = nonVisualDrawingProperties99.Description;
                            //if (desc == null || desc.Value == null || !desc.Value.Contains(alternateTextToFind))
                            //    continue;

                            P.BlipFill blipFill = picture.Descendants<P.BlipFill>().First();
                            var blip = blipFill.Descendants<DocumentFormat.OpenXml.Drawing.Blip>().First();
                            string embedId = blip.Embed; // now we need to find the embedded content and update it. 

                            // find the content 
                            ImagePart imagePart = (ImagePart)slide.GetPartById(embedId);
                            if (imagePart != null)
                            {
                                try
                                {
                                    using (FileStream fileStream = new FileStream(newImagePath + LstShowPicture[Convert.ToInt32(desc)], FileMode.Open))
                                    {
                                        imagePart.FeedData(fileStream);
                                        fileStream.Close();
                                    }
                                }
                                catch (Exception ex)
                                {
                                    try
                                    {
                                        using (FileStream fileStream = new FileStream(newImagePath + newImage, FileMode.Open))
                                        {
                                            imagePart.FeedData(fileStream);
                                            fileStream.Close();
                                        }
                                    }
                                    catch (Exception ex1)
                                    {

                                    }
                                }
                            }
                        }

                        // String Text
                        foreach (string StringText in LstShowText)
                        {
                            string[] Value = StringText.Split(';');
                            List<DocumentFormat.OpenXml.Drawing.Text> textList = slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>().ToList();
                            foreach (DocumentFormat.OpenXml.Drawing.Text txt in textList)
                            {
                                foreach (PPTXTemplateFields pptx in PPTXAllparams)
                                {
                                    string SelectedField = pptx.SelectedField.Trim().Substring(0, pptx.SelectedField.Trim().Length - 1);
                                    try
                                    {
                                        if (txt.Text.Contains(SelectedField + (TextIndex + 1).ToString()))
                                        {
                                            if (pptx.FieldDataType == "String" || pptx.FieldDataType == "Int")
                                            {
                                                int Index = FieldPosition(dtFieldActive, pptx.MappedFields);
                                                //if (Index != -1) txt.Text = txt.Text.Replace(SelectedField + (TextIndex + 1).ToString(), Value[Index]); else txt.Text = " ";
                                                if (Index != -1) txt.Text = txt.Text.SafeReplace(SelectedField + (TextIndex + 1).ToString(), Value[Index], true); else txt.Text = " ";
                                            }
                                            else if (pptx.FieldDataType == "Float")
                                            {
                                                int Index = FieldPosition(dtFieldActive, pptx.MappedFields);
                                                if (Index != -1)
                                                {
                                                    try
                                                    {
                                                        //txt.Text = txt.Text.Replace(SelectedField + (TextIndex + 1).ToString(), Convert.ToDecimal(Value[Index]).ToString("#,##0.00"));
                                                        txt.Text = txt.Text.SafeReplace(SelectedField + (TextIndex + 1).ToString(), Convert.ToDecimal(Value[Index]).ToString("#,##0.00"), true);
                                                    }
                                                    catch (Exception ex)
                                                    {
                                                        txt.Text = " ";
                                                    }
                                                }
                                                else txt.Text = " ";
                                            }
                                            else if (pptx.FieldDataType == "DateTime")
                                            {
                                                int Index = FieldPosition(dtFieldActive, pptx.MappedFields);
                                                if (Index != -1)
                                                {
                                                    //txt.Text = txt.Text.Replace(SelectedField + (TextIndex + 1).ToString(), Convert.ToDateTime(Value[Index]).ToString(pptx.FieldDataFormat, CultureInfo.InvariantCulture).ToString());
                                                    txt.Text = txt.Text.SafeReplace(SelectedField + (TextIndex + 1).ToString(), Convert.ToDateTime(Value[Index]).ToString(pptx.FieldDataFormat, CultureInfo.InvariantCulture).ToString(), true);
                                                }
                                                else txt.Text = " ";
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {

                                    }
                                }
                            }
                            TextIndex++;
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        // Replace text in PPT slide
        private void ReplaceTextMatchingAltText(PresentationDocument presentationDocument, string relationshipId)
        {
            OpenXmlElementList slideIds = presentationDocument.PresentationPart.Presentation.SlideIdList.ChildElements;

            foreach (SlideId sID in slideIds) // loop thru the SlideIDList
            {
                string relId = sID.RelationshipId; // get first slide relationship
                if (relationshipId == relId)
                {
                    SlidePart slide = (SlidePart)presentationDocument.PresentationPart.GetPartById(relId); // Get the slide part from the relationship ID. 

                    P.ShapeTree tree = slide.Slide.CommonSlideData.ShapeTree;
                    foreach (P.Shape shape in tree.Elements<P.Shape>())
                    {
                        // Run through all the paragraphs in the document
                        foreach (A.Paragraph paragraph in shape.Descendants().OfType<A.Paragraph>())
                        {
                            foreach (A.Run run in paragraph.Elements<A.Run>())
                            {
                                if (run.Text.InnerText.Contains("Name"))
                                {
                                    run.Text = new A.Text("Your new text");
                                }
                            }
                        }
                    }
                }
            }
        }

        // Method get all Text box in a Node of PPTX slide 
        public string GetNodeAllTextBoxes(string FName, HttpPostedFileBase File)
        {
            int Idx = 0;
            string Fields = "";

            // Get the complete folder path and store the file inside it.  
            FName = System.IO.Path.Combine(HttpContext.Current.Server.MapPath("~/Content/PPTX"), FName);
            File.SaveAs(FName);
            using (var presentationDocument = PresentationDocument.Open(FName, true))
            {
                PresentationPart presentationPart = presentationDocument.PresentationPart;
                OpenXmlElementList slideIds = presentationDocument.PresentationPart.Presentation.SlideIdList.ChildElements;
                foreach (SlideId sID in slideIds) // loop thru the SlideIDList
                {
                    string relId = sID.RelationshipId; // get first slide relationship
                    SlidePart slide = (SlidePart)presentationDocument.PresentationPart.GetPartById(relId);

                    List<DocumentFormat.OpenXml.Drawing.Text> textList = slide.Slide.Descendants<DocumentFormat.OpenXml.Drawing.Text>().ToList();
                    foreach (DocumentFormat.OpenXml.Drawing.Text txt in textList)
                    {
                        if (txt.Text.Contains("1"))
                        {
                            string input = txt.Text;
                            string output = input.Substring(input.IndexOf(':') + 1);
                            if (output.Contains("1"))
                            {
                                if (!Fields.Contains(output))
                                    Fields += "," + output;
                            }
                        }

                        Idx++;
                    }
                    break;
                }
            }

            return Fields;
        }

        /// <summary> 
        /// Insert Image into Slide 
        /// </summary> 
        /// <param name="filePath">PowerPoint Path</param> 
        /// <param name="imagePath">Image Path</param> 
        /// <param name="imageExt">Image Extension</param> 
        public void InsertImageInSlide(Slide slide, string imagePath, string imageExt)
        {
            // Creates an Picture instance and adds its children. 
            P.Picture picture = new P.Picture();
            string embedId = string.Empty;
            embedId = "rId" + (slide.Elements().Count() + 915).ToString();
            P.NonVisualPictureProperties nonVisualPictureProperties = new P.NonVisualPictureProperties(
                new P.NonVisualDrawingProperties() { Id = (UInt32Value)4U, Name = "Picture 5" },
                new P.NonVisualPictureDrawingProperties(new A.PictureLocks() { NoChangeAspect = true }),
                new ApplicationNonVisualDrawingProperties());


            P.BlipFill blipFill = new P.BlipFill();
            Blip blip = new Blip() { Embed = embedId };


            // Creates an BlipExtensionList instance and adds its children 
            BlipExtensionList blipExtensionList = new BlipExtensionList();
            BlipExtension blipExtension = new BlipExtension() { Uri = "{28A0092B-C50C-407E-A947-70E740481C1C}" };


            UseLocalDpi useLocalDpi = new UseLocalDpi() { Val = false };
            useLocalDpi.AddNamespaceDeclaration("a14",
                "http://schemas.microsoft.com/office/drawing/2010/main");


            blipExtension.Append(useLocalDpi);
            blipExtensionList.Append(blipExtension);
            blip.Append(blipExtensionList);


            Stretch stretch = new Stretch();
            FillRectangle fillRectangle = new FillRectangle();
            stretch.Append(fillRectangle);


            blipFill.Append(blip);
            blipFill.Append(stretch);


            // Creates an ShapeProperties instance and adds its children. 
            P.ShapeProperties shapeProperties = new P.ShapeProperties();


            A.Transform2D transform2D = new A.Transform2D();
            A.Offset offset = new A.Offset() { X = 457200L, Y = 1524000L };
            A.Extents extents = new A.Extents() { Cx = 8229600L, Cy = 5029200L };


            transform2D.Append(offset);
            transform2D.Append(extents);


            A.PresetGeometry presetGeometry = new A.PresetGeometry() { Preset = A.ShapeTypeValues.Rectangle };
            A.AdjustValueList adjustValueList = new A.AdjustValueList();


            presetGeometry.Append(adjustValueList);


            shapeProperties.Append(transform2D);
            shapeProperties.Append(presetGeometry);


            picture.Append(nonVisualPictureProperties);
            picture.Append(blipFill);
            picture.Append(shapeProperties);


            slide.CommonSlideData.ShapeTree.AppendChild(picture);


            // Generates content of imagePart. 
            ImagePart imagePart = slide.SlidePart.AddNewPart<ImagePart>(imageExt, embedId);
            FileStream fileStream = new FileStream(imagePath, FileMode.Open);
            imagePart.FeedData(fileStream);
            fileStream.Close();
        }
    }
}