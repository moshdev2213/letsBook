package com.example.letsbook.ModalDao;

public class ReservationReq {
    private String trainId;
    private String scheduleId;
    private String email;
    private String reserved;
    private int seats;

    public ReservationReq(String trainId, String scheduleId, String email, String reserved, int seats) {
        this.trainId = trainId;
        this.scheduleId = scheduleId;
        this.email = email;
        this.reserved = reserved;
        this.seats = seats;
    }

    public String getTrainId() {
        return trainId;
    }

    public void setTrainId(String trainId) {
        this.trainId = trainId;
    }

    public String getScheduleId() {
        return scheduleId;
    }

    public void setScheduleId(String scheduleId) {
        this.scheduleId = scheduleId;
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
