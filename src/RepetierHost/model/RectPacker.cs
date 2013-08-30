using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RepetierHost.model
{
    public class PackerPos
    {
        public int x, y;
        public PackerPos() { }
        public PackerPos(int _x, int _y)
        {
            x = _x;
            y = _y;
        }
        public bool isEqual(PackerPos obj)
        {
            return x == obj.x && y == obj.y;
        }
    }

    public class PackerRect : PackerPos
    {
        public int w, h;
        public object obj;
        public PackerRect(int _x, int _y, int _w, int _h, object _obj)
        {
            x = _x;
            y = _y;
            w = _w;
            h = _h;
            obj = _obj;
        }
        public bool containsPoint(PackerPos p)
        {
            return (p.x >= x && p.y >= y &&
                p.x < (x + w) && p.y < (y + h));
        }
        public bool containsRect(PackerRect r)
        {
            return (r.x >= x && r.y >= y &&
                (r.x + r.w) <= (x + w) && (r.y + r.h) <= (y + h));
        }
        public bool intersects(PackerRect r)
        {
            return w > 0 && h > 0 && r.w > 0 && r.h > 0 &&
                ((r.x + r.w) > x && r.x < (x + w) &&
                    (r.y + r.h) > y && r.y < (y + h));
        }

        //  Greater rect area. Not as good as the next heuristic
        //  static bool Greater(const TRect &a, const TRect &b) { return a.w*a.h > b.w*b.h; }

        // Greater size in at least one dim.
        public static bool greater(PackerRect a, PackerRect b)
        {
            return (a.w > b.w && a.w > b.h) ||
            (a.h > b.w && a.h > b.h);
        }

    }

    public class RectPacker
    {
        PackerRect size;
        public List<PackerRect> vRects;
        List<PackerPos> vPositions;
        long area;
        // ----------------------------------------------------------------------------------------
        // Name        : RectPlacement.cpp
        // Description : A class that fits subrectangles into a power-of-2 rectangle
        //               (C) Copyright 2000-2002 by Javier Arevalo
        //               This code is free to use and modify for all purposes
        // ----------------------------------------------------------------------------------------

        /*
         You have a bunch of rectangular pieces. You need to arrange them in a 
         rectangular surface so that they don't overlap, keeping the total area of the 
         rectangle as small as possible. This is fairly common when arranging characters 
         in a bitmapped font, lightmaps for a 3D engine, and I guess other situations as 
         well.
 
         The idea of this algorithm is that, as we add rectangles, we can pre-select 
         "interesting" places where we can try to add the next rectangles. For optimal 
         results, the rectangles should be added in order. I initially tried using area 
         as a sorting criteria, but it didn't work well with very tall or very flat 
         rectangles. I then tried using the longest dimension as a selector, and it 
         worked much better. So much for intuition...
 
         These "interesting" places are just to the right and just below the currently 
         added rectangle. The first rectangle, obviously, goes at the top left, the next 
         one would go either to the right or below this one, and so on. It is a weird way 
         to do it, but it seems to work very nicely.
 
         The way we search here is fairly brute-force, the fact being that for most off-
         line purposes the performance seems more than adequate. I have generated a 
         japanese font with around 8500 characters and all the time was spent generating 
         the bitmaps.
 
         Also, for all we care, we could grow the parent rectangle in a different way 
         than power of two. It just happens that power of 2 is very convenient for 
         graphics hardware textures.
 
         I'd be interested in hearing of other approaches to this problem. Make sure
         to post them on http://www.flipcode.com
         */


        public RectPacker(int _w, int _h)
        {
            size = new PackerRect(0, 0, _w, _h, null);
            vRects = new List<PackerRect>();
            vPositions = new List<PackerPos>();
            vPositions.Add(new PackerPos(0, 0));
            area = 0;
        }
        void end()
        {
            vPositions.Clear();
            vRects.Clear();
            size.w = 0;
        }

        public bool isOK
        {
            get
            {
                return size.w > 0;
            }
        }
        public int w { get { return size.w; } }
        public int h { get { return size.h; } }
        public long totalArea
        {
            get { return size.w * size.h; }
        }

        // --------------------------------------------------------------------------------
        // Name        : IsFree
        // Description : Check if the given rectangle is partially or totally used
        // --------------------------------------------------------------------------------
        public bool isFree(PackerRect r)
        {
            if (!size.containsRect(r))
                return false;
            foreach (PackerRect it in vRects)
                if (it.intersects(r))
                    return false;
            return true;
        }


        // --------------------------------------------------------------------------------
        // Name        : AddPosition
        // Description : Add new anchor point
        // --------------------------------------------------------------------------------
        public void addPosition(PackerPos p)
        {
            // Try to insert anchor as close as possible to the top left corner
            // So it will be tried first
            bool bFound = false;
            int pos = 0;
            foreach (PackerPos it in vPositions)
            {
                if (p.x + p.y < it.x + it.y)
                {
                    bFound = true;
                    break;
                }
                pos++;
            }
            if (bFound)
                vPositions.Insert(pos, p);
            else
                vPositions.Add(p);
        }

        // --------------------------------------------------------------------------------
        // Name        : AddRect
        // Description : Add the given rect and updates anchor points
        // --------------------------------------------------------------------------------
        public void addRect(PackerRect r)
        {
            vRects.Add(r);
            area += r.w * r.h;

            // Add two new anchor points
            addPosition(new PackerPos(r.x, r.y + r.h));
            addPosition(new PackerPos(r.x + r.w, r.y));
        }

        // --------------------------------------------------------------------------------
        // Name        : AddAtEmptySpot
        // Description : Add the given rectangle
        // --------------------------------------------------------------------------------
        public bool addAtEmptySpot(PackerRect r)
        {
            // Find a valid spot among available anchors.

            bool bFound = false;
            int pos = 0;
            foreach (PackerPos it in vPositions)
            {
                PackerRect rect = new PackerRect(it.x, it.y, r.w, r.h, r.obj);

                if (isFree(rect))
                {
                    r = rect;
                    bFound = true;
                    break; // Don't let the loop increase the iterator.
                }
                pos++;
            }
            if (bFound)
            {
                // Remove the used anchor point
                vPositions.RemoveAt(pos);

                // Sometimes, anchors end up displaced from the optimal position
                // due to irregular sizes of the subrects.
                // So, try to adjut it up & left as much as possible.
                int x, y;
                for (x = 1; x <= r.x; x++)
                    if (!isFree(new PackerRect(r.x - x, r.y, r.w, r.h, r.obj)))
                        break;
                for (y = 1; y <= r.y; y++)
                    if (!isFree(new PackerRect(r.x, r.y - y, r.w, r.h, r.obj)))
                        break;
                if (y > x)
                    r.y -= y - 1;
                else
                    r.x -= x - 1;
                addRect(r);
            }
            return bFound;
        }


        // --------------------------------------------------------------------------------
        // Name        : AddAtEmptySpotAutoGrow
        // Description : Add a rectangle of the given size, growing our area if needed
        //               Area grows only until the max given.
        //               Returns the placement of the rect in the rect's x,y coords
        // --------------------------------------------------------------------------------

        public bool addAtEmptySpotAutoGrow(PackerRect pRect, int maxW, int maxH)
        {
            if (pRect.w <= 0)
                return true;

            int orgW = size.w;
            int orgH = size.h;

            // Try to add it in the existing space
            while (!addAtEmptySpot(pRect))
            {
                int pw = size.w;
                int ph = size.h;

                // Sanity check - if area is complete.
                if (pw >= maxW && ph >= maxH)
                {
                    size.w = orgW;
                    size.h = orgH;
                    return false;
                }

                // Try growing the smallest dim
                if (pw < maxW && (pw < ph || ((pw == ph) && (pRect.w >= pRect.h))))
                    size.w = Math.Min(maxW, pw + 10); //*2;
                else
                    size.h = Math.Min(maxH, ph + 10); //*2;
                if (addAtEmptySpot(pRect))
                    break;

                // Try growing the other dim instead
                if (pw != size.w)
                {
                    size.w = pw;
                    if (ph < maxW)
                        size.h = Math.Min(maxH, ph + 10); //*2;
                }
                else
                {
                    size.h = ph;
                    if (pw < maxW)
                        size.w = Math.Min(maxW, pw + 10); //*2;
                }

                if (pw != size.w || ph != size.h)
                    if (addAtEmptySpot(pRect))
                        break;

                // Grow both if possible, and reloop.
                size.w = pw;
                size.h = ph;
                if (pw < maxW)
                    size.w = Math.Min(maxW, pw + 10); //*2;
                if (ph < maxH)
                    size.h = Math.Min(maxH, ph + 10); //*2;
            }
            return true;
        }
    }
}
