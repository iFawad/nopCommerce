/*
** custom js functions
*/

function displayPopupContentFromUrlYesNoAgeVerification(url, title, modal, yesText, noText, width) {
  var isModal = (modal ? true : false);
  var targetWidth = (width ? width : 550);
  var maxHeight = $(window).height() - 20;

  $('<div></div>').load(url)
    .dialog({
      modal: isModal,
      width: targetWidth,
      maxHeight: maxHeight,
      title: title,
      buttons: [{
        text: yesText,
        click: function (event, ui) {
          localStorage.setItem('advertOnce', 'true');
          $(this).dialog('destroy').remove();
        }
      },
      {
        text: noText,
        click: function (event, ui) {
          window.location.href = 'http://google.com/';
          $(this).dialog('destroy').remove();
        }
      }],
      open: function () {                      // open event handler
        $(this)                               // the element being dialogged
          .parent()                          // get the dialog widget element
          .find(".ui-dialog-titlebar-close")
          .hide();
      }
    });
}