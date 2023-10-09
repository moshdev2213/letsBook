package com.example.letsbook.Modal;

import java.io.Serializable;

public class CheckRes implements Serializable {
    private String selectedDate;
    private String selectedSeat;
    private String selectedShedule;
    private String sheduleId;

    public CheckRes(String selectedDate, String selectedSeat, String selectedShedule,String sheduleId) {
        this.selectedDate = selectedDate;
        this.selectedSeat = selectedSeat;
        this.selectedShedule = selectedShedule;
        this.sheduleId = sheduleId;
    }

    public String getSheduleId() {
        return sheduleId;
    }

    public void setSheduleId(String sheduleId) {
        this.sheduleId = sheduleId;
    }

    public String getSelectedDate() {
        return selectedDate;
    }

    public void setSelectedDate(String selectedDate) {
        this.selectedDate = selectedDate;
    }

    public String getSelectedSeat() {
        return selectedSeat;
    }

    public void setSelectedSeat(String selectedSeat) {
        this.selectedSeat = selectedSeat;
    }

    public String getSelectedShedule() {
        return selectedShedule;
    }

    public void setSelectedShedule(String selectedShedule) {
        this.selectedShedule = selectedShedule;
    }
}
