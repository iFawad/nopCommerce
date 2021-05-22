/*
** extension to custom js functions
*/

function displayPopupContentFromUrlYesNoAgeVerification(url, title, modal, width) {
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
        text: "Yes, I'm 21+",
        click: function (event, ui) {
          localStorage.setItem('advertOnce', 'true');
          $(this).dialog('destroy').remove();
        }
      },
      {
        text: "No, I'm under 21",
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