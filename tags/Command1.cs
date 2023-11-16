#region Namespaces
using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

#endregion

namespace tags
{
    [Transaction(TransactionMode.Manual)]
    public class Command1 : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            // this is a variable for the Revit application
            UIApplication uiapp = commandData.Application;

            // this is a variable for the current Revit model
            Document doc = uiapp.ActiveUIDocument.Document;

            View curview = doc.ActiveView;
            

            Transaction t = new Transaction(doc);

            t.Start("Creating Tags");
            {
                int count = 0;
                if (curview.ViewType == ViewType.FloorPlan)
                {

                    ViewType viewType = curview.ViewType;
                    Dictionary<ViewType, List<BuiltInCategory>> elemnts = getdictionary(viewType);
                    List<BuiltInCategory> categories = new List<BuiltInCategory>();
                    if (elemnts.TryGetValue(viewType, out categories)) { }
                    ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(categories);
                    FilteredElementCollector viewelements = new FilteredElementCollector(doc, curview.Id).WherePasses(catfilter).WhereElementIsNotElementType();


                    foreach (Element curelement in viewelements)
                    {
                        XYZ insPoint = getinsertionpoint(curelement);
                        if(insPoint == null) 
                        { 
                            continue; 
                        }

                        if (curelement.Category.Name != "Walls")
                        {
                            if(curelement.Category.Name == "Rooms")
                            {
                                double newY = insPoint.Y + 3.00;
                                XYZ a = new XYZ(insPoint.X, newY, insPoint.Z);
                                Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                                FamilySymbol curtagtype = tags[curelement.Category.Name];
                                Reference curref = new Reference(curelement);
                                IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, a);

                            }
                            else
                            {
                                Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                                FamilySymbol curtagtype = tags[curelement.Category.Name];
                                Reference curref = new Reference(curelement);
                                IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, insPoint);
                            }
                        }
 
                        if (curelement.Category.Name == "Walls")
                        {
                            Wall curwall = curelement as Wall;
                            WallType curwalltype = curwall.WallType;
                            if(curwalltype.Kind == WallKind.Curtain)
                            {
                                Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                                FamilySymbol curtagtype = tags[curelement.Category.Name];
                                Reference curref = new Reference(curelement);
                                IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, insPoint);
                            }
                            else
                            {
                                Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                                FamilySymbol curtagtype = tags[curelement.Category.Name];
                                Reference curref = new Reference(curelement);
                                IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, insPoint);
                            }
                        }
                        count++;
                    }
                }
                if (curview.ViewType == ViewType.CeilingPlan)
                {
                    ViewType viewType = curview.ViewType;
                    Dictionary<ViewType, List<BuiltInCategory>> elemnts = getdictionary(viewType);
                    List<BuiltInCategory> categories = new List<BuiltInCategory>();
                    if (elemnts.TryGetValue(viewType, out categories)) { }
                    ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(categories);
                    FilteredElementCollector viewelements = new FilteredElementCollector(doc, curview.Id).WherePasses(catfilter).WhereElementIsNotElementType();

                    foreach (Element curelement in viewelements)
                    {
                        // for getting an insertion point
                        XYZ insPoint = getinsertionpoint(curelement);
                        if (insPoint == null)
                        {
                            continue;
                        }
                        // for getting an insertion point

                        Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                        FamilySymbol curtagtype = tags[curelement.Category.Name];
                        Reference curref = new Reference(curelement);

                        if (curelement.Category.Name != "Areas")
                        {
                            IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, insPoint);
                        }
                        count++;
                    }
                }

                if (curview.ViewType == ViewType.AreaPlan)
                {
                    ViewType viewType = curview.ViewType;
                    //curview.SetDetailLevel = ViewDetailLevel.Fine;
                    Dictionary<ViewType, List<BuiltInCategory>> elemnts = getdictionary(viewType);
                    List<BuiltInCategory> categories = new List<BuiltInCategory>();
                    if (elemnts.TryGetValue(viewType, out categories)) { }
                    ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(categories);
                    FilteredElementCollector viewelements = new FilteredElementCollector(doc, curview.Id).WherePasses(catfilter).WhereElementIsNotElementType();

                    foreach (Element curelement in viewelements)
                    {
                        // for getting an insertion point
                        XYZ insPoint = getinsertionpoint(curelement);
                        if (insPoint == null)
                        {
                            continue;
                        }
                        // for getting an insertion point
                        Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                        FamilySymbol curtagtype = tags[curelement.Category.Name];
                        Reference curref = new Reference(curelement);
                        if (curelement.Category.Name == "Areas")
                        {
                            ViewPlan curareaplan = curview as ViewPlan;
                            Area curarea = curelement as Area;
                            AreaTag curareatag = doc.Create.NewAreaTag(curareaplan, curarea, new UV(insPoint.X, insPoint.Y));
                            curareatag.TagHeadPosition = new XYZ(insPoint.X, insPoint.Y, 0);
                        }
                        count++;
                    }
                }
                if (curview.ViewType == ViewType.Section)
                {
                    ViewType viewType = curview.ViewType;
                    Dictionary<ViewType, List<BuiltInCategory>> elemnts = getdictionary(viewType);
                    List<BuiltInCategory> categories = new List<BuiltInCategory>();
                    if (elemnts.TryGetValue(viewType, out categories)) { }
                    ElementMulticategoryFilter catfilter = new ElementMulticategoryFilter(categories);
                    FilteredElementCollector viewelements = new FilteredElementCollector(doc, curview.Id).WherePasses(catfilter).WhereElementIsNotElementType();

                    foreach (Element curelement in viewelements)
                    {
                        // for getting an insertion point
                        XYZ insPoint = getinsertionpoint(curelement);
                        if (insPoint == null)
                        {
                            continue;
                        }
                        // for getting an insertion point

                        Dictionary<string, FamilySymbol> tags = getfamilysymbol(doc);
                        FamilySymbol curtagtype = tags[curelement.Category.Name];
                        Reference curref = new Reference(curelement);
                        double newZ = insPoint.Z + 3.00;
                        XYZ a = new XYZ(insPoint.X, insPoint.Y, newZ);
                        if(elementtaggedornot(curview,curelement) == false)
                        {
                            if (curelement.Category.Name == "Rooms")
                            {
                                IndependentTag curtag = IndependentTag.Create(doc, curtagtype.Id, curview.Id, curref, false, TagOrientation.Horizontal, a);
                                count++;
                            }
                        }

                        
                    }
                }
                TaskDialog.Show("Tags", $"{count} tags were placed");
            }

            t.Commit();
            t.Dispose();
            // Your code goes here


            return Result.Succeeded;
        }

        private Dictionary<string, FamilySymbol> getfamilysymbol(Document doc)
        {
            Dictionary<string, FamilySymbol> tag = new Dictionary<string, FamilySymbol>();
            tag.Add("Walls", getsymname(doc, "M_Wall Tag"));
            tag.Add("Areas", getsymname(doc, "M_Area Tag"));
            tag.Add("Curtain Walls", getsymname(doc, "M_Curtain Wall Tag"));
            tag.Add("Doors", getsymname(doc, "M_Door Tag"));
            tag.Add("Furniture", getsymname(doc, "M_Furniture Tag"));
            tag.Add("Lighting Fixtures", getsymname(doc, "M_Lighting Fixture Tag"));
            tag.Add("Rooms", getsymname(doc, "M_Room Tag"));
            tag.Add("Windows", getsymname(doc, "M_Window Tag"));
            return tag;
        }

        private FamilySymbol getsymname(Document doc, string familyname)
        {
            FamilySymbol tagname = new FilteredElementCollector(doc).OfClass(typeof(FamilySymbol)).Cast<FamilySymbol>().Where(x => x.FamilyName.Equals(familyname)).First();
            return tagname;
        }

        private Dictionary<ViewType, List<BuiltInCategory>> getdictionary(ViewType viewType)
        {
            Dictionary<ViewType, List<BuiltInCategory>> Dict = new Dictionary<ViewType, List<BuiltInCategory>>();
            Dict.Add(ViewType.FloorPlan, new List<BuiltInCategory> { BuiltInCategory.OST_Walls, BuiltInCategory.OST_Doors, BuiltInCategory.OST_Rooms, BuiltInCategory.OST_Windows, BuiltInCategory.OST_Furniture });
            Dict.Add(ViewType.CeilingPlan, new List<BuiltInCategory> { BuiltInCategory.OST_Rooms, BuiltInCategory.OST_LightingFixtures });
            Dict.Add(ViewType.AreaPlan, new List<BuiltInCategory> { BuiltInCategory.OST_Areas });
            Dict.Add(ViewType.Section, new List<BuiltInCategory> { BuiltInCategory.OST_Rooms });
            return Dict;
        }

        private XYZ getinsertionpoint(Element curelement)
        {
            XYZ insPoint;
            LocationPoint curpoint;
            LocationCurve curcurve;

            Location loc = curelement.Location;
            if (loc != null)
            {

                curpoint = loc as LocationPoint;
                if (curpoint != null)
                {
                    insPoint = curpoint.Point;
                }
                else
                {
                    curcurve = loc as LocationCurve;
                    Curve cur = curcurve.Curve;
                    insPoint = getmidpointofcurve(cur.GetEndPoint(0), cur.GetEndPoint(1));
                }
                return insPoint;
            }
            return null;
        }

        private bool elementtaggedornot(View curview, Element curelement)
        {
            FilteredElementCollector elementtag = new FilteredElementCollector(curelement.Document, curview.Id).OfClass(typeof(IndependentTag)).WhereElementIsNotElementType();
            foreach (IndependentTag curtag in elementtag)
            {
                List<ElementId> list = curtag.GetTaggedLocalElementIds().ToList();
                foreach (ElementId elementid in list)
                {
                    if (elementid == curelement.Id)
                    {
                        return true;
                    }

                }

            }
            return false;
        }

        private XYZ getmidpointofcurve(XYZ pt1, XYZ pt2)
        {
            XYZ a = new XYZ((pt1.X + pt2.X) / 2, (pt1.Y + pt2.Y) / 2, (pt1.Z + pt2.Z) / 2);
            return a;
        }

        internal static PushButtonData GetButtonData()
        {
            // use this method to define the properties for this command in the Revit ribbon
            string buttonInternalName = "btnCommand1";
            string buttonTitle = "Button 1";

            ButtonDataClass myButtonData1 = new ButtonDataClass(
                buttonInternalName,
                buttonTitle,
                MethodBase.GetCurrentMethod().DeclaringType?.FullName,
                Properties.Resources.Blue_32,
                Properties.Resources.Blue_16,
                "This is a tooltip for Button 1");

            return myButtonData1.Data;
        }
    }
}
