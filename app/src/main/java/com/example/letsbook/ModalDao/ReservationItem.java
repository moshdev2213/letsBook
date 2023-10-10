package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class ReservationItem implements Serializable {
    private String id;
    private String collectionId;
    private String collectionName;
    private String created;
    private String updated;
    private String trainId;
    private String sheduleId;
    private String reserved;
    private int seats;
    private String email;
    private int canceled;


    public int getCanceled() {
        return canceled;
    }

    public void setCanceled(int canceled) {
        this.canceled = canceled;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getCollectionId() {
        return collectionId;
    }

    public void setCollectionId(String collectionId) {
        this.collectionId = collectionId;
    }

    public String getCollectionName() {
        return collectionName;
    }

    public void setCollectionName(String collectionName) {
        this.collectionName = collectionName;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }

    public String getUpdated() {
        return updated;
    }

    public void setUpdated(String updated) {
        this.updated = updated;
    }

    public String getTrainId() {
        return trainId;
    }

    public void setTrainId(String trainId) {
        this.trainId = trainId;
    }

    public String getSheduleId() {
        return sheduleId;
    }

    public void setSheduleId(String sheduleId) {
        this.sheduleId = sheduleId;
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

    public String getEmail() {
        return email;
    }

    public void setEmail(String email) {
        this.email = email;
    }

    public ReservationItem(String id, String collectionId, String collectionName, String created, String updated, String trainId, String sheduleId, String reserved, int seats, String email,int canceled) {
        this.id = id;
        this.collectionId = collectionId;
        this.collectionName = collectionName;
        this.created = created;
        this.updated = updated;
        this.trainId = trainId;
        this.sheduleId = sheduleId;
        this.reserved = reserved;
        this.seats = seats;
        this.email = email;
        this.canceled = canceled;
    }
}
