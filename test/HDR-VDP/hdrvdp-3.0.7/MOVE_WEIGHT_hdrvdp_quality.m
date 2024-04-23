function X = MOVE_WEIGHT_hdrvdp_quality ()

root = "test_move_weights";
test_number = 4;
dirs = [root+"/Test-"+test_number+"-SP/"+"0.33_0.34_0.33", ...
        root+"/Test-"+test_number+"-SP/"+"0.5_0.5_0", ...
        root+"/Test-"+test_number+"-SP/"+"0.5_0_0.5", ...
        root+"/Test-"+test_number+"-SP/"+"0_0.5_0.5", ...
        root+"/Test-"+test_number+"-SP/"+"0_0_1", ...
        root+"/Test-"+test_number+"-SP/"+"0_1_0", ...
        root+"/Test-"+test_number+"-SP/"+"1_0_0", ...
        root+"/Test-"+test_number+"-AOI"];
fullDir = root+"/FullMoving";

% Display parameters
Y_peak = 200;     % Peak luminance in cd/m^2 (the same as nit)
contrast = 1000;  % Display contrast 1000:1
gamma = 2.2;      % Standard gamma-encoding
E_ambient = 100;  % Ambient light = 100 lux

imgs = dir(fullDir + "/screen*.png");
imgs = {imgs(:).name};
[imgs, ~] = sort_nat(imgs);
perspNum = length(imgs);
fulls = cell(perspNum, 1);
for i = 1:perspNum
    full = double(imread(fullDir + "/" + imgs(i))) / (2^8-1);
    full = hdrvdp_gog_display_model(full, Y_peak, contrast, gamma, E_ambient );
    fulls{i} = full;
end

for i = 1:length(dirs)
    f = figure('visible','off');
    d1 = dirs(i);
    X = zeros(perspNum, 1);
    mbps = ["/4", "/10", "/20", "/40"];
    for k = 1:length(mbps)
        d = d1 + mbps(k) + "/SCREEN/";
        imgs = dir(d + "/screen*.png");
        imgs = {imgs(:).name};
        [imgs, ~] = sort_nat(imgs);
        if exist(d1 + mbps(k) + "/vdp-hdr-quality", 'dir')
            rmdir(d1 + mbps(k) + "/vdp-hdr-quality", 's');
        end
        mkdir(d1 + mbps(k) + "/vdp-hdr-quality");
        for j = 1:length(imgs)
            img = double(imread(d + imgs(j))) / (2^8-1);
            img = hdrvdp_gog_display_model(img, Y_peak, contrast, gamma, E_ambient );
            diff = hdrvdp3('quality', img, fulls{j}, 'rgb-native', 30, []);
            imagesc(diff.Q);
            clim([0,1]);
            colorbar;
            saveas(f, d1 + mbps(k) + "/vdp-hdr-quality/" + imgs(j));
            X(j) = diff.Q;
            disp(dirs(i) + " - Directory " + mbps(k) + " ->" + j + ": " + imgs(j) + ": " + X(j));
        end
        plot(X)
        delete(d1 + mbps(k) + "/vdp-hdr-quality.png");
        delete(d1 + mbps(k) + "/vdp-hdr-quality.xlsx");
        saveas(f, d1 + mbps(k) + "/vdp-hdr-quality.png");
        writematrix(X, d1 + mbps(k) + "/vdp-hdr-quality.xlsx");
    end
end
