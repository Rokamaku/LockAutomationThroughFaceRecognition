export function showGrowlMessage(sender, messageSeverity, messageSummary, messageDetail) {
  sender.props.showGrowl({
    life: 2000,
    severity: messageSeverity, 
    summary: messageSummary, 
    detail: messageDetail 
  });
}

export function showSuccessMessage(sender, messageSummary, messageDetail) {
  showGrowlMessage(sender, 'success', messageSummary, messageDetail)
}

export function showInfoMessage(sender, messageSummary, messageDetail) {
  showGrowlMessage(sender, 'info', messageSummary, messageDetail)
}

export function showWarningMessage(sender, messageSummary, messageDetail) {
  showGrowlMessage(sender, 'warn', messageSummary, messageDetail)
}

export function showErrorMessage(sender, messageSummary, messageDetail) {
  showGrowlMessage(sender, 'error', messageSummary, messageDetail)
}