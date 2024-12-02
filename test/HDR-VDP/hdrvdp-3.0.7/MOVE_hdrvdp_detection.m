function X = MOVE_hdrvdp_detection()

root = "test_move";
dirs = [root+"/Test-2-SP", root+"/Test-2-AOI"];
%dirs = [root+"/Test-1-SP"];
%dirs = [root+"/Test-AOI"];
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
disp("perspNum is " + perspNum);
fulls = cell(perspNum, 1);
for i = 1:perspNum
    full = double(imread(fullDir + "/" + imgs(i))) / (2^8-1);
    full = hdrvdp_gog_display_model(full, Y_peak, contrast, gamma, E_ambient );
    %disp(full)
    %disp(class(full));
    %disp(class(fulls));
    fulls{i} = double(full);
end

[height, width, ~] = size(imread(fullDir + "/" + imgs(1)));
ppd = hdrvdp_pix_per_deg(30, [width height], 0.5 );
disp("ppd is " + ppd);

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
        if exist(d1 + mbps(k) + "/vdp-hdr-detection", 'dir')
            rmdir(d1 + mbps(k) + "/vdp-hdr-detection", 's');
        end
        mkdir(d1 + mbps(k) + "/vdp-hdr-detection");
        for j = 1:length(imgs)
            img = double(imread(d + imgs(j))) / (2^8-1);
            img = hdrvdp_gog_display_model(img, Y_peak, contrast, gamma, E_ambient );
            diff = hdrvdp3('detection', img, fulls{j}, 'rgb-native', ppd, []);
            imagesc(diff.P_map);
            clim([0,1]);
            colorbar;
            saveas(f, d1 + mbps(k) + "/vdp-hdr-detection/" + imgs(j));
            X(j) = mean(diff.P_map, "all");
            disp(dirs(i) + " - Directory " + mbps(k) + " ->" + j + ": " + imgs(j) + ": " + X(j));
        end
        plot(X)
        delete(d1 + mbps(k) + "/vdp-hdr-detection.png");
        delete(d1 + mbps(k) + "/vdp-hdr-detection.xlsx");
        saveas(f, d1 + mbps(k) + "/vdp-hdr-detection.png");
        writematrix(X, d1 + mbps(k) + "/vdp-hdr-detection.xlsx");
    end
end
