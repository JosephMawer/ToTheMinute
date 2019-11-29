# 2TheMinute
Simple app used for tracking time


todo:

1) extract list view items into a control
2) make such that each list view item (lvi) displays the correct week
2) Define look and data for list box view
3) set up bindings to each list view items
4) Calculate Time for the entire day

#future work
- could be cool to have notifications, such as: notify user is still clocked in for certain categories; reminders to clock out


########## OUTLINE OF LIST VIEW ITEM OBJECT AKA, TimeSheetViewModel ########

float TotalMinutes = get_total_minutes();
bool ClockIn 
string Category


// handles clocking in and out; currently code is in code behind
command ClockInCommand {get;set;}
// calculates minutes/hours for the previous week or the next week and updates the week component
command GetNextOrPreviousWeekCommand {get;set;}


##pseudo code for calculating hours for each day of the week

categoryList = select distinct Categories from *TableName*
foreach category : categoryList  select * from _TableName_ where Category = 'Category' and Date >= 'Calculate days for this week

######### END OF OUTLINE ##########################