using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace FinancialDatabase
{
    public abstract class ControlLabelPair : Label
    {
        protected Control control;
        public bool inEditMode { get; private set; }

        protected string _attrib;
        public string attrib
        {
            get { return _attrib; }
            set
            {
                // Only set attrib once for any object, should never need to change it
                if (_attrib != null) { throw new Exception("Error ControlLabelPair: Trying to set attrib when attrib is already set"); }

                _attrib = value;
            }
        }


        public ControlLabelPair()
        {
            inEditMode = false;

            ParentChanged += ParentChangedMethod;
            this.LocationChanged += UpdatedAttributes;
            this.VisibleChanged += VisibilityChangedMethod;
        }

        public abstract void updateControlValWithLabelText();

        public void setEditMode(bool newInEditMode)
        {
            if (newInEditMode)
            {
                editMode();
            } else
            {
                viewMode();
            }
        }


        internal void ParentChangedMethod(Object e, EventArgs ea)
        {
            if (Parent != null)
            {
                Parent.Controls.Add(control);
            }
        }


        protected abstract void UpdatedAttributes(Object e, EventArgs ea);
        /*{
            label.Location = new Point(this.Location.X, this.Location.Y);
            label.Size = new Size(100, 30);
            label.BackColor = Color.Transparent;
            label.Name = "Label-" + this.Name;
            label.Visible = true;
        }*/


        internal void VisibilityChangedMethod(Object e, EventArgs ea)
        {
            if (control is null) throw new Exception("Error ControlLabelPair: Trying to change control visibility when label does not exist yet, meaning this object was not added to its parent's control");
            control.Visible = !this.Visible;
        }


        public abstract object getControlValue();
        /*{
            if (control is null) throw new Exception("Error ControlLabelPair: Trying to access control value when control does not exist yet, meaning this object was not added to its parent's controls");
            return label.Text;
        }*/
        public abstract string getControlValueAsStr();

        public string getLabelText()
        {
            return this.Text;
        }

        public void setLabelText(string newText)
        {
            this.Text = newText;
        }

        public abstract bool userChangedValue();

        public abstract void setControlVal(object val);
        public abstract void setControlVal(string val);

        public void flipEditMode()
        {
            if (inEditMode)
            {
                viewMode();
            }
            else
            {
                editMode();
            }
        }


        public void editMode()
        {
            if (control is null) throw new Exception("Error ControlLabelPair: Trying to switch into edit mode when control does not exist yet, meaning this object was not added to its parent's controls");

            control.Visible = true;
            this.Visible = false;
            inEditMode = true;

            setControlVal(getLabelText());
        }


        public void viewMode()
        {
            if (control is null) throw new Exception("Error ControlLabelPair: Trying to switch into view mode when control does not exist yet, meaning this object was not added to its parent's controls");
            control.Visible = false;
            this.Visible = true;
            inEditMode = false;

            this.Text = getControlValueAsStr();
        }
    }
}
