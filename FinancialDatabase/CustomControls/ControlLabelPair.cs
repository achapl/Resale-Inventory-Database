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
    /// <summary>
    /// A ControlLabelPair (CLP) is a Label that has its own control
    /// The control is there to store a user-inputted value when in "edit" mode
    /// 
    /// The control value will not automatically update the Label value when switched back into "view" mode
    /// since the user may not have inputted a valid value into the control
    /// 
    /// Implementation of changing a user-value into what the label displays is left to the inherited class
    /// </summary>
    public abstract class ControlLabelPair : Label
    {
        protected Control control;

        public bool inEditMode { get; private set; }

        
        /// <summary>
        /// attrib is a value the bound to the CLP so CLP's
        /// can each be associated with a specific database column
        /// </summary>
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
            this.LocationChanged += initializeControl;
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


        /// <summary>
        /// Information for the control object can only be set once the label is set up
        /// Once Label is set up (attribute is updated), the control object can be initialized
        /// 
        /// This is because the control's location is set relative to the label's location
        /// </summary>
        protected abstract void initializeControl(Object _, EventArgs __);


        internal void VisibilityChangedMethod(Object e, EventArgs ea)
        {
            if (control is null) { return; }
            control.Visible = !this.Visible;
        }


        public abstract object getControlValue();


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
        }


        public void viewMode()
        {
            if (control is null) throw new Exception("Error ControlLabelPair: Trying to switch into view mode when control does not exist yet, meaning this object was not added to its parent's controls");
            control.Visible = false;
            this.Visible = true;
            inEditMode = false;
        }
    }
}
