import axios from 'axios'


class ImageDataAccess {
    constructor(){
        this.image_axios = axios.create({
            baseURL: 'https://localhost:5401/api/Image',
            headers: { 'Content-Type': 'application/json' }
        });
    }

    async getImage(guid, loadtags) {
        return await this.image_axios.get(guid, {
            params: {
                loadTags: loadtags
            }
        }).then(res => res.data);
    }

    async getImages(loadtags, name, description, tagName) {
        const response = await this.image_axios.get('', {
            params: {
                loadTags: loadtags,
                name: name,
                description: description,
                tagName: tagName
            } 
        }
        ).then(res => res.data);
        return response;
    }

    async getLegalFileExtensions() {
        const response = await this.image_axios.get('extenstions', {}).then(res => res.data);
        return response;
    }

    async addImage(image) {
        let message = "Image added succesfully";
        await this.image_axios.post('', image)
            .catch(function (error) {
                message = "Image not added succesfully. Please try again.";
            })
            .then(() =>
                alert(message)
            );
    }

    async patchImage(image, updateTags) {
        let message = "Image updated succesfully";
        await this.image_axios.patch('', image, { params: { updateTags: updateTags } })
            .catch(function (error) {
                message = "Image not updated succesfully. Please try again.";
            }).then(()=>
                alert(message)  
            )
    }

    async deleteImage(guid) {
        let message = "Image removed succesfully.";
        await this.image_axios.delete(guid, {})
            .catch(function (error) {
                message = "Image not removed succesfully. Please try again.";
        })
        .then(() =>
            alert(message)
        );
    }
}

export default ImageDataAccess;