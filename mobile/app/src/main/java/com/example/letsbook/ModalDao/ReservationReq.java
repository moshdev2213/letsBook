package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class ReservationReq implements Serializable {
    private String trainId;
    private String sheduleId;
    private String email;
    private String reserved;
    private int canceled;
    private int seats;

    public ReservationReq(String trainId, String scheduleId, String email, String reserved, int seats,int canceled) {
        this.trainId = trainId;
        this.sheduleId = scheduleId;
        this.email = email;
        this.reserved = reserved;
        this.seats = seats;
        this.canceled = canceled;
    }

    public int getCanceled() {
        return canceled;
    }

    public void setCanceled(int canceled) {
        this.canceled = canceled;
    }

    public String getTrainId() {
        return trainId;
    }

    public void setTrainId(String trainId) {
        this.trainId = trainId;
    }

    public String getScheduleId() {
        return sheduleId;
    }

    public void setScheduleId(String scheduleId) {
        this.sheduleId = scheduleId;
    }

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public String getReserved() {
        return reserved;
    }

    public void setReserved(String reserved) {
        this.reserved = reserved;
    }

    public int getSeats() {
        return seats;
    }

    public void setSeats(int seats) {
        this.seats = seats;
    }
}
