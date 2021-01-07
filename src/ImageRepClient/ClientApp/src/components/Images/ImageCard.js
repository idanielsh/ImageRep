import React, { Component } from 'react';

class ImageCard extends Component {
    render() {
        let alt = this.props.description === "" ? this.props.name : this.props.description;
        return (
            <div className="card">
                <img className="card-img-top img-fluid image-card-picture" src={"data:image/"+this.props.name.substring(this.props.name.lastIndexOf(".")+ 1) + ";base64," + this.props.picture} alt={alt}/>
                <div className="card-body">
                    <h5 className="card-title limit-lines">{this.props.name}</h5>
                    <p className="card-text limit-lines">{this.props.description}</p>
                    <div className="row">
                        <div className="col-6 pr-1 form-group">
                            <button className="btn btn-outline-danger btn-block" onClick={(e) => this.props.deleteImage(e, this.props.id)} >Delete</button>
                        </div>
                        <div className="col-6 pl-0 form-group">
                            <a href={"/edit/"+this.props.id} className="btn btn-outline-primary btn-block">More Info</a>
                        </div>
                    </div>
                    
                </div>
            </div>
            );
    }
}

export default ImageCard;