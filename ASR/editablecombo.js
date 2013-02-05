/*----------- edit control -------------*/

//Common functions for all dropdowns

/*----------------------------------------------
The Common functions used for all dropdowns are:
-----------------------------------------------
-- function fnKeyDownHandler(getdropdown, e)
-- function fnLeftToRight(getdropdown)
-- function fnRightToLeft(getdropdown)
-- function fnDelete(getdropdown)
-- function FindKeyCode(e)
-- function FindKeyChar(e)
-- function fnSanityCheck(getdropdown)

--------------------------- Subrata Chakrabarty */

function fnKeyDownHandler(getdropdown, e) {
    fnSanityCheck(getdropdown);

    // Press [ <- ] and [ -> ] arrow keys on the keyboard to change alignment/flow.
    // ...go to Start : Press  [ <- ] Arrow Key
    // ...go to End : Press [ -> ] Arrow Key
    // (this is useful when the edited-text content exceeds the ListBox-fixed-width)
    // This works best on Internet Explorer, and not on Netscape

    var vEventKeyCode = FindKeyCode(e);

    // Press left/right arrow keys
    if (vEventKeyCode == 37) {
        fnLeftToRight(getdropdown);
    }
    if (vEventKeyCode == 39) {
        fnRightToLeft(getdropdown);
    }

    // Delete key pressed
    if (vEventKeyCode == 46) {
        fnDelete(getdropdown);
    }

    // backspace key pressed
    if (vEventKeyCode == 8 || vEventKeyCode == 127) {
        if (e.which) //Netscape
        {
            //e.which = ''; //this property has only a getter.
        }
        else //Internet Explorer
        {
            //To prevent backspace from activating the -Back- button of the browser
            e.keyCode = '';
            if (window.event.keyCode) {
                window.event.keyCode = '';
            }
        }
        return true;
    }

    // Tab key pressed, use code below to reorient to Left-To-Right flow, if needed
    //if(vEventKeyCode == 9)
    //{
    //  fnLeftToRight(getdropdown);
    //}
}

function fnLeftToRight(getdropdown) {
    getdropdown.style.direction = "ltr";
}

function fnRightToLeft(getdropdown) {
    getdropdown.style.direction = "rtl";
}

function fnDelete(getdropdown) {
    if (getdropdown.options.length != 0)
    // if dropdown is not empty
    {
        if (getdropdown.options.selectedIndex == vEditableOptionIndex_A)
        // if option the Editable field
        {
            getdropdown.options[getdropdown.options.selectedIndex].text = '';
            getdropdown.options[getdropdown.options.selectedIndex].value = '';
        }
    }
}


/*
Since Internet Explorer and Netscape have different
ways of returning the key code, displaying keys
browser-independently is a bit harder.
However, you can create a script that displays keys
for either browser.
The following function will display each key
in the status line:

The "FindKey.." function receives the "event" object
from the event handler and stores it in the variable "e".
It checks whether the "e.which" property exists (for Netscape),
and stores it in the "keycode" variable if present.
Otherwise, it assumes the browser is Internet Explorer
and assigns to keycode the "e.keyCode" property.
*/

function FindKeyCode(e) {
    if (e.which) {
        keycode = e.which;  //Netscape
    }
    else {
        keycode = e.keyCode; //Internet Explorer
    }

    //alert("FindKeyCode"+ keycode);
    return keycode;
}

function FindKeyChar(e) {
    keycode = FindKeyCode(e);
    if ((keycode == 8) || (keycode == 127)) {
        character = "backspace"
    }
    else if ((keycode == 46)) {
        character = "delete"
    }
    else {
        character = String.fromCharCode(keycode);
    }
    //alert("FindKey"+ character);
    return character;
}

function fnSanityCheck(getdropdown) {
    if (vEditableOptionIndex_A > (getdropdown.options.length - 1)) {
        alert("PROGRAMMING ERROR: The value of variable vEditableOptionIndex_... cannot be greater than (length of dropdown - 1)");
        return false;
    }
}

//Dropdown specific functions, which manipulate dropdown specific global variables

/*----------------------------------------------
Dropdown specific global variables are:
-----------------------------------------------
1) vEditableOptionIndex_A   --> this needs to be set by Programmer!! See explanation.
2) vEditableOptionText_A    --> this needs to be set by Programmer!! See explanation.
3) vPreviousSelectIndex_A
4) vSelectIndex_A
5) vSelectChange_A

--------------------------- Subrata Chakrabarty */

/*----------------------------------------------
The dropdown specific functions
(which manipulate dropdown specific global variables)
used by all dropdowns are:
-----------------------------------------------
1) function fnChangeHandler_A(getdropdown)
2) function fnKeyPressHandler_A(getdropdown, e)
3) function fnKeyUpHandler_A(getdropdown, e)

--------------------------- Subrata Chakrabarty */

/*------------------------------------------------
IMPORTANT: Global Variable required to be SET by programmer
-------------------------- Subrata Chakrabarty  */

var vEditableOptionIndex_A = 0;

// Give Index of Editable option in the dropdown.
// For eg.
// if first option is editable then vEditableOptionIndex_A = 0;
// if second option is editable then vEditableOptionIndex_A = 1;
// if third option is editable then vEditableOptionIndex_A = 2;
// if last option is editable then vEditableOptionIndex_A = (length of dropdown - 1).
// Note: the value of vEditableOptionIndex_A cannot be greater than (length of dropdown - 1)

var vEditableOptionText_A = "--?--";

// Give the default text of the Editable option in the dropdown.
// For eg.
// if the editable option is <option ...>--?--</option>,
// then set vEditableOptionText_A = "--enter field name--";

/*------------------------------------------------
Global Variables required for
fnChangeHandler_A(), fnKeyPressHandler_A() and fnKeyUpHandler_A()
for Editable Dropdowns
-------------------------- Subrata Chakrabarty  */

var vPreviousSelectIndex_A = 0;
// Contains the Previously Selected Index, set to 0 by default

var vSelectIndex_A = 0;
// Contains the Currently Selected Index, set to 0 by default

var vSelectChange_A = 'MANUAL_CLICK';
// Indicates whether Change in dropdown selected option
// was due to a Manual Click
// or due to System properties of dropdown.

// vSelectChange_A = 'MANUAL_CLICK' indicates that
// the jump to a non-editable option in the dropdown was due
// to a Manual click (i.e.,changed on purpose by user).

// vSelectChange_A = 'AUTO_SYSTEM' indicates that
// the jump to a non-editable option was due to System properties of dropdown
// (i.e.,user did not change the option in the dropdown;
// instead an automatic jump happened due to inbuilt
// dropdown properties of browser on typing of a character )

/*------------------------------------------------
Functions required for  Editable Dropdowns
-------------------------- Subrata Chakrabarty  */

function fnChangeHandler_A(getdropdown) {
    fnSanityCheck(getdropdown);

    vPreviousSelectIndex_A = vSelectIndex_A;
    // Contains the Previously Selected Index

    vSelectIndex_A = getdropdown.options.selectedIndex;
    // Contains the Currently Selected Index

    if ((vPreviousSelectIndex_A == (vEditableOptionIndex_A)) && (vSelectIndex_A != (vEditableOptionIndex_A)) && (vSelectChange_A != 'MANUAL_CLICK'))
    // To Set value of Index variables - Subrata Chakrabarty
    {
        getdropdown[(vEditableOptionIndex_A)].selected = true;
        vPreviousSelectIndex_A = vSelectIndex_A;
        vSelectIndex_A = getdropdown.options.selectedIndex;
        vSelectChange_A = 'MANUAL_CLICK';
        // Indicates that the Change in dropdown selected
        // option was due to a Manual Click
    }
    var header = document.getElementById("ColumnHeader");
    header.value = getdropdown.options[getdropdown.selectedIndex].text.replace(/(.)([A-Z])/g, "$1 $2");
}


function fnKeyPressHandler_A(getdropdown, e) {
    fnSanityCheck(getdropdown);

    keycode = FindKeyCode(e);
    keychar = FindKeyChar(e);

    // Check for allowable Characters
    // The various characters allowable for entry into Editable option..
    // may be customized by minor modifications in the code (if condition below)
    // (you need to know the keycode/ASCII value of the  character to be allowed/disallowed.
    // - Subrata Chakrabarty

    if ((keycode > 47 && keycode < 59) || (keycode > 62 && keycode < 127) || (keycode == 32)) {
        var vAllowableCharacter = "yes";
    }
    else {
        var vAllowableCharacter = "no";
    }

    //alert(window); alert(window.event);

    if (getdropdown.options.length != 0)
    // if dropdown is not empty
        if (getdropdown.options.selectedIndex == (vEditableOptionIndex_A))
    // if selected option the Editable option of the dropdown
    {

        var vEditString = getdropdown[vEditableOptionIndex_A].value;

        // make Editable option Null if it is being edited for the first time
        if ((vAllowableCharacter == "yes") || (keychar == "backspace")) {
            if (vEditString == vEditableOptionText_A)
                vEditString = "";
        }
        if (keychar == "backspace")
        // To handle backspace - Subrata Chakrabarty
        {
            vEditString = vEditString.substring(0, vEditString.length - 1);
            // Decrease length of string by one from right

            vSelectChange_A = 'MANUAL_CLICK';
            // Indicates that the Change in dropdown selected
            // option was due to a Manual Click

        }
        //alert("EditString2:"+vEditString);

        if (vAllowableCharacter == "yes")
        // To handle addition of a character - Subrata Chakrabarty
        {
            vEditString += String.fromCharCode(keycode);
            // Concatenate Enter character to Editable string

            // The following portion handles the "automatic Jump" bug
            // The "automatic Jump" bug (Description):
            //   If a alphabet is entered (while editing)
            //   ...which is contained as a first character in one of the read-only options
            //   ..the focus automatically "jumps" to the read-only option
            //   (-- this is a common property of normal dropdowns
            //    ..but..is undesirable while editing).

            var i = 0;
            var vEnteredChar = String.fromCharCode(keycode);
            var vUpperCaseEnteredChar = vEnteredChar;
            var vLowerCaseEnteredChar = vEnteredChar;


            if (((keycode) >= 97) && ((keycode) <= 122))
            // if vEnteredChar lowercase
                vUpperCaseEnteredChar = String.fromCharCode(keycode - 32);
            // This is UpperCase


            if (((keycode) >= 65) && ((keycode) <= 90))
            // if vEnteredChar is UpperCase
                vLowerCaseEnteredChar = String.fromCharCode(keycode + 32);
            // This is lowercase

            if (e.which) //For Netscape
            {
                // Compare the typed character (into the editable option)
                // with the first character of all the other
                // options (non-editable).

                // To note if the jump to the non-editable option was due
                // to a Manual click (i.e.,changed on purpose by user)
                // or due to System properties of dropdown
                // (i.e.,user did not change the option in the dropdown;
                // instead an automatic jump happened due to inbuilt
                // dropdown properties of browser on typing of a character )

                for (i = 0; i <= (getdropdown.options.length - 1); i++) {
                    if (i != vEditableOptionIndex_A) {
                        var vReadOnlyString = getdropdown[i].value;
                        var vFirstChar = vReadOnlyString.substring(0, 1);
                        if ((vFirstChar == vUpperCaseEnteredChar) || (vFirstChar == vLowerCaseEnteredChar)) {
                            vSelectChange_A = 'AUTO_SYSTEM';
                            // Indicates that the Change in dropdown selected
                            // option was due to System properties of dropdown
                            break;
                        }
                        else {
                            vSelectChange_A = 'MANUAL_CLICK';
                            // Indicates that the Change in dropdown selected
                            // option was due to a Manual Click
                        }
                    }
                }
            }
        }

        // Set the new edited string into the Editable option
        getdropdown.options[vEditableOptionIndex_A].text = vEditString;
        getdropdown.options[vEditableOptionIndex_A].value = vEditString;

        return false;
    }
    return true;
}

function fnKeyUpHandler_A(getdropdown, e) {
    fnSanityCheck(getdropdown);

    if (e.which) // Netscape
    {
        if (vSelectChange_A == 'AUTO_SYSTEM') {
            // if editable dropdown option jumped while editing
            // (due to typing of a character which is the first character of some other option)
            // then go back to the editable option.
            getdropdown[(vEditableOptionIndex_A)].selected = true;
        }

        var vEventKeyCode = FindKeyCode(e);
        // if [ <- ] or [ -> ] arrow keys are pressed, select the editable option
        if ((vEventKeyCode == 37) || (vEventKeyCode == 39)) {
            getdropdown[vEditableOptionIndex_A].selected = true;
        }
    }
}
