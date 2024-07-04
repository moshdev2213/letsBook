package com.example.letsbook.ModalDao;

import java.io.Serializable;

public class TrainItem implements Serializable {
    private String id;
    private String toStation;
    private String fromStation;
    private String trainType;
    private String created;
    private String trainName;
    private int seat;

    public TrainItem(String id, String toStation, String fromStation, String trainType, String created,String trainName,int seat) {
        this.id = id;
        this.toStation = toStation;
        this.fromStation = fromStation;
        this.trainType = trainType;
        this.created = created;
        this.trainName = trainName;
        this.seat = seat;
    }

    public int getSeat() {
        return seat;
    }

    public void setSeat(int seat) {
        this.seat = seat;
    }

    public String getTrainName() {
        return trainName;
    }

    public void setTrainName(String trainName) {
        this.trainName = trainName;
    }

    public String getId() {
        return id;
    }

    public void setId(String id) {
        this.id = id;
    }

    public String getToStation() {
        return toStation;
    }

    public void setToStation(String toStation) {
        this.toStation = toStation;
    }

    public String getFromStation() {
        return fromStation;
    }

    public void setFromStation(String fromStation) {
        this.fromStation = fromStation;
    }

    public String getTrainType() {
        return trainType;
    }

    public void setTrainType(String trainType) {
        this.trainType = trainType;
    }

    public String getCreated() {
        return created;
    }

    public void setCreated(String created) {
        this.created = created;
    }
}
