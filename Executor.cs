using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;

public class Executor
{
    public static void ExecuteDirectionDefiner(Element startingEleemnt)
    {
        // Executed elements, to avoid repetitions.
        ICollection<Element> elementColl = new List<Element>();
        // Results.
        List<PipingSection> pipingSections = new List<PipingSection>();
        // startingEleemnt - starting point, first element in the model.
        MarkElement(startingEleemnt, elementColl, pipingSections);
        // Check if there's only one element in the model.
        if (pipingSections.Count == 1)
        {
            PipingSection pipingSection = pipingSections.First();
            pipingSection.Direction = PipeDirection.UNDIRECTED;
            SetDirection(pipingSection);
        }
        else
        {
            foreach (PipingSection pipingSection in pipingSections)
            {
                try
                {
                    if (pipingSection.IsVertical() || pipingSection.IsSloped())
                    {
                        PipingSection currentPipingSection = pipingSection;
                        PipingSection previousPipingSection = null;
                        XYZ currentPoint = GetLocationPoint(currentPipingSection);
                        XYZ previousPoint = null;
                        PipeDirection direction;
                        // Checking to avoid out of range exception for first element in the collection.
                        if (pipingSection.Equals(pipingSections.First()))
                        {
                            previousPipingSection = pipingSections.First(ps => ps.Section == pipingSection.Section + 1);
                            previousPoint = GetLocationPoint(previousPipingSection);
                            direction = currentPoint.Z < previousPoint.Z ?
                                PipeDirection.UP :
                                PipeDirection.DOWN;
                        }
                        // Checking to avoid out of range exception for last element in the collection.
                        else if (pipingSection.Equals(pipingSections.Last()))
                        {
                            previousPipingSection = pipingSections.First(ps => ps.Section == pipingSection.Section - 1);
                            previousPoint = GetLocationPoint(previousPipingSection);
                            direction = currentPoint.Z < previousPoint.Z ?
                                PipeDirection.DOWN :
                                PipeDirection.UP;
                        }
                        else
                        {
                            previousPipingSection = pipingSections.ElementAt(pipingSections.IndexOf(pipingSection) + 1);
                            previousPoint = GetLocationPoint(previousPipingSection);
                            direction = currentPoint.Z < previousPoint.Z ?
                                PipeDirection.UP :
                                PipeDirection.DOWN;
                        }
                        // Setting direction to the current pipingSection.
                        pipingSection.Direction = direction;
                    }
                    else
                        // All the horizontal piping sections don't have direction.
                        pipingSection.Direction = PipeDirection.UNDIRECTED;
                    // Setting directions to the model.
                    SetDirection(pipingSection);
                }
                catch { }
            }
        }
    }

    private static void MarkElement(Element element, ICollection<Element> elements, List<PipingSection> pipingSections)
    {
        // Checking if it's first time using.
        if (!elements.Select(e => e.Id).Contains(element.Id))
        {
            pipingSections.Add(new PipingSection(element) { Section = elements.Count });
            elements.Add(element);
            Element currentElement = null;
            // Get all the connected pipes in the model.
            ICollection<Element> connectedElements = GetConnectedElements(element);
            if (connectedElements.Count() == 1)
            {
                currentElement = connectedElements.First();
                if (!elements.Select(e => e.Id).Contains(currentElement.Id))
                {
                    MarkElement(currentElement, elements, pipingSections);
                }
            }
            else
            {
                foreach (Element connectedElement in connectedElements)
                {
                    if (!elements.Select(e => e.Id).Contains(connectedElement.Id))
                    {
                        MarkElement(connectedElement, elements, pipingSections);
                    }
                }
            }
        }
    }
}